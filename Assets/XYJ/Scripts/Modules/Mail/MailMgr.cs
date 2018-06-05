#if !USE_HOT
using NetProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xys.hot.UI;
using xys.UI;

namespace xys.hot
{
    public class MailMgr
    {
        public C2WMailRequest worldRequest { get; protected set; }
        public void OnInit()
        {
            hotApp.my.handler.Reg<NetProto.Int64>(Protoid.W2C_MailSyncMaxMailId, OnSyncMaxMailId);
            hotApp.my.handler.Reg<NetProto.MailFetchAttachmentResult>(Protoid.W2C_MailFetchAttachmentResult, OnFetchAttachmentResult);
            hotApp.my.eventSet.Subscribe(EventID.FinishLoadScene, () => { this.QueryMails(m_localMaxMailId + 1); });
            MainPanelItemListener listener = new MainPanelItemListener(() => { return true; }, () => { return this.newMailFlag; });
            xys.hot.UI.MainPanel.SetItemListener((int)PanelType.UIFriendPanel, listener);
        }

        public void OnAwake()
        {
            worldRequest = new C2WMailRequest(App.my.world.local);
        }

        bool m_newMailFlag;
        public bool newMailFlag
        {
            get { return m_newMailFlag; }
            set
            {
                m_newMailFlag = value;
                App.my.eventSet.fireEvent(EventID.Mail_NewMailFlagChange);
                MainPanel.SetItemReadyActive((int)PanelType.UIWelfarePanel, value);
            }
        }
        long m_remoteMaxMailId = 0;
        long m_localMaxMailId = 0;
        private void OnSyncMaxMailId(NetProto.Int64 input)
        {
            long maxMailId = input.value;
            if (maxMailId > m_remoteMaxMailId)
            {
                m_remoteMaxMailId = maxMailId;
            }
            if (m_remoteMaxMailId > m_localMaxMailId)
            {
                this.QueryMails(m_localMaxMailId + 1);
            }
        }
        private void OnFetchAttachmentResult(MailFetchAttachmentResult input)
        {
            if (m_currBatchFetchMailId == input.mailId)
            {
                this.SetReadMark(m_currBatchFetchMailId);   
                if (input.isFetchAttachmentVal)
                {
                    m_currBatchFetchMailId = 0;
                    this.BatchFetchAttachment(null);
                }
                else
                {
                    this.StopBatchFetchAttachment();
                }
            }
            if (0 != input.mailId)
            {
                if (m_mails.ContainsKey(input.mailId))
                {
                    m_mails[input.mailId].isFetchedAttachments = input.isFetchAttachmentVal;
                }
                hotApp.my.eventSet.FireEvent<MailFetchAttachmentResult>(EventID.Mail_AttachmentFetchStateChange, input);
            }
            if (MailError.MailError_PackageFull == input.ret)
                xys.UI.Utility.TipContentUtil.Show("mail_package_full");
        }

        private Dictionary<long, MailDbRecord> m_mails = new Dictionary<long, MailDbRecord>();
        private HashSet<long> m_removedMailHistory = new HashSet<long>();

        public Dictionary<long, MailDbRecord> mails { get { return m_mails; } }

        public void SetReadMark(long mailId)
        {
            worldRequest.SetReadMark(new NetProto.Int64() { value = mailId }, (wProtobuf.RPC.Error code)=> { });

            MailDbRecord mail = null;
            if (m_mails.TryGetValue(mailId, out mail))
            {
                mail.isRead = true;
            }
        }
        public void FetchAttachment(long mailId)
        {
            worldRequest.FetchAttachment(new NetProto.Int64() { value = mailId }, (wProtobuf.RPC.Error code) => { });
        }

        private long m_currBatchFetchMailId = 0;
        private HashSet<long> m_bathFetchMailIds = new HashSet<long>();
        private long m_lastTryBatchFetchSec = 0;
        public void BatchFetchAttachment(HashSet<long> mailIds)
        {
            if (MailDef.GetNowSecond() > m_lastTryBatchFetchSec + 10) // 基本不可能
            {
                this.StopBatchFetchAttachment();
            }
            if (null != mailIds)
            {
                m_bathFetchMailIds.UnionWith(mailIds);
            }
            if (0 == m_currBatchFetchMailId && m_bathFetchMailIds.Count > 0)
            {
                m_lastTryBatchFetchSec = MailDef.GetNowSecond();
                m_currBatchFetchMailId = m_bathFetchMailIds.Min();
                m_bathFetchMailIds.Remove(m_currBatchFetchMailId);
                worldRequest.FetchAttachment(new NetProto.Int64() { value = m_currBatchFetchMailId }, (wProtobuf.RPC.Error code) => { });
            }
        }

        public void StopBatchFetchAttachment()
        {
            m_currBatchFetchMailId = 0;
            m_bathFetchMailIds.Clear();
        }

        public void RemoveMail(long mailId)
        {
            worldRequest.RemoveMail(new NetProto.Int64() { value = mailId }, (wProtobuf.RPC.Error code) => { });
            m_mails.Remove(mailId);
            m_removedMailHistory.Add(mailId);
        }

        public void QueryMails(long beginMailId)
        {
            worldRequest.QueryMails(new NetProto.Int64() { value = beginMailId}, 
                (wProtobuf.RPC.Error code, MailRecords mailRecords) => { this.OnRspQueryMails(code, mailRecords, beginMailId); });
        }
        private void OnRspQueryMails(wProtobuf.RPC.Error code, MailRecords mailRecords, long beginMailId)
        {
            if (wProtobuf.RPC.Error.Success != code)
                return;

            if (beginMailId <= 0)
            {
                m_mails.Clear();
            }
            if (mailRecords.records.Count > 0)
            {
                long maxId = 0;
                foreach (var mail in mailRecords.records)
                {
                    if (mail.id > maxId)
                        maxId = mail.id;

                    if (!m_removedMailHistory.Contains(mail.id))
                        m_mails[mail.id] = mail;
                }
                QueryMails(maxId + 1);
            }
            else
            {
                this.CheckRemoveMail();
            }
        }

        bool m_firstCheckRemoveMail = true;
        private void CheckRemoveMail()
        {
            long nowTick = MailDef.GetNowTick();
            if (m_mails.Count > 0)
            {
                List<long> removeMails = new List<long>();
                foreach (var mail in m_mails.Values)
                {
                    if (MailDef.IsMailExpired(nowTick, mail.createTick))
                        removeMails.Add(mail.id);
                }
                foreach (var mail in removeMails)
                {
                    m_mails.Remove(mail);
                }
            }
            if (m_mails.Count > MailDef.MAIL_SAVE_MAX_COUNT)
            {
                Comparison<MailDbRecord> comparison = new Comparison<MailDbRecord>(
                   (MailDbRecord x, MailDbRecord y) =>
                   {
                       if (x.createTick != y.createTick)
                           return x.createTick > y.createTick ? -1 : 1;
                       return x.id > y.id ? -1 : 1;
                   });
                List<MailDbRecord> mails = m_mails.Values.ToList();
                mails.Sort(comparison);
                mails = mails.GetRange(0, MailDef.MAIL_SAVE_MAX_COUNT);
                m_mails.Clear();
                foreach (var mail in mails)
                    m_mails[mail.id] = mail;
            }

            // 第一次拉取邮件，把未读邮件当作新邮件
            // 以后的拉去，以出现mailId > m_localMailId判定拉到新邮件
            if (m_firstCheckRemoveMail)
            {
                m_firstCheckRemoveMail = false;

                bool hasUnreadMail = false;
                foreach (var mail in m_mails.Values)
                {
                    if (!mail.isRead)
                    {
                        hasUnreadMail = true;
                        break;
                    }
                }
                newMailFlag = hasUnreadMail;
            }
            else
            {
                long maxMailId = 0;
                foreach (var mailId in m_mails.Keys)
                {
                    if (maxMailId < mailId)
                        maxMailId = mailId;
                }
                if (maxMailId > m_localMaxMailId)
                    newMailFlag = true;
                m_localMaxMailId = maxMailId;
            }
        }
    }
}
#endif