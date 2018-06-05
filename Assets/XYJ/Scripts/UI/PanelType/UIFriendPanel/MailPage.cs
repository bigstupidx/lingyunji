#if !USE_HOT
using Config;
using NetProto;
using NetProto.Hot;
using System;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;
using xys.UI.Utility;

namespace xys.hot.UI
{
    namespace Friend
    {
        class MailPage : HotTablePageBase
        {
            MailPage() : base(null) { }
            MailPage(HotTablePage page) : base (page)
            {

            }

            [SerializeField]
            StateRoot m_pageState;
            [SerializeField]
            Text m_countPromptTxt;
            [SerializeField]
            Button m_quickFetchBtn;
            [SerializeField]
            Button m_clearAllBtn;
            [SerializeField]
            Text m_titleTxt;
            [SerializeField]
            Text m_contentTxt;
            [SerializeField]
            Text m_senderTxt;
            [SerializeField]
            Button m_fetchBtn;
            [SerializeField]
            Button m_deleteBtn;
            [SerializeField]
            StateRoot m_attachmentState;
            [SerializeField]
            GameObject m_attachmentItemPrefab;
            [SerializeField]
            GameObject m_mailItemPrefab;
            [SerializeField]
            Transform m_mailItemContainer;
            [SerializeField]
            Transform m_attachmentItemContainer;
            [SerializeField]
            ListViewMailItem m_listViewMailItem;

            protected override void OnInit()
            {
                m_listViewMailItem.SelectItemHandler += M_listViewMailItem_SelectItemHandler;
                m_fetchBtn.onClick.AddListenerIfNoExist(OnClickFetchBtn);
                m_deleteBtn.onClick.AddListenerIfNoExist(OnClickDeleteBtn);
                m_quickFetchBtn.onClick.AddListenerIfNoExist(OnClickQuickFetchBtn);
                m_clearAllBtn.onClick.AddListenerIfNoExist(OnClearAllReadMail);
            }

            private void OnClickFetchBtn()
            {
                if (null == m_mailData || null == m_mailData.mail ||
                    null == m_mailData.mail.attachments || 
                    m_mailData.mail.attachments.Count <= 0 ||
                    m_mailData.mail.isFetchedAttachments)
                {
                    return;
                }

                MailDef.mailMgr.FetchAttachment(m_mailData.id);
            }

            private void OnClickDeleteBtn()
            {
                if (null == m_mailData || null == m_mailData.mail)
                    return;

                if (MailDef.HasAttachement(m_mailData.mail) && !m_mailData.mail.isFetchedAttachments)
                {
                    if (null == m_twoBtn)
                    {
                        m_twoBtn = xys.UI.Dialog.TwoBtn.Show(
                            "", TipContentUtil.GenText("mail_remove_mail"),
                            TipContentUtil.GenText("cancel"), () => false,
                            TipContentUtil.GenText("comfirm"), () =>
                            {
                                MailDef.mailMgr.RemoveMail(m_mailData.id);
                                this.ResetUI();
                                return false;
                            }, true, true, () => { m_twoBtn = null; });
                    }
                }
                else
                {
                    MailDef.mailMgr.RemoveMail(m_mailData.id);
                    this.ResetUI();
                }
            }
            private void OnClickQuickFetchBtn()
            {
                HashSet<long> mailIds = new HashSet<long>();
                foreach (var itemData in m_listViewMailItem.DataSource)
                {
                    if (null != itemData &&
                        MailDef.HasAttachement(itemData.mail) &&
                        !itemData.mail.isFetchedAttachments)
                    {
                        mailIds.Add(itemData.id);
                    }
                }
                if (mailIds.Count <= 0)
                {
                    TipContentUtil.Show("mail_no_attachment_to_fetch");
                }
                else
                {
                    MailDef.mailMgr.BatchFetchAttachment(mailIds);
                }
            }

            xys.UI.Dialog.TwoBtn m_twoBtn;
            private void OnClearAllReadMail()
            {
                List<long> removeIds = new List<long>();
                foreach (var itemData in m_listViewMailItem.DataSource)
                {
                    if (null != itemData && null != itemData.mail)
                    {
                        bool canRemove = false;
                        if (itemData.mail.isFetchedAttachments)
                            canRemove = true;
                        if (itemData.mail.isRead && !MailDef.HasAttachement(itemData.mail))
                            canRemove = true;
                        if (canRemove)
                        {
                            removeIds.Add(itemData.id);
                        }
                    }
                }
                if (removeIds.Count > 0)
                {
                    if (null == m_twoBtn)
                    {
                        m_twoBtn = xys.UI.Dialog.TwoBtn.Show(
                            "", TipContentUtil.GenText("mail_remove_all_read_mail"),
                            TipContentUtil.GenText("cancel"), () => false,
                            TipContentUtil.GenText("comfirm"), () =>
                            {
                                foreach (var mailId in removeIds)
                                {
                                    MailDef.mailMgr.RemoveMail(mailId);
                                }
                                this.ResetUI();
                                return false;
                            }, true, true, () => { m_twoBtn = null; });
                    }
                }
                else
                {
                    TipContentUtil.Show("mail_not_removable_mail");
                }
            }

            private void M_listViewMailItem_SelectItemHandler(ListViewMailItem listView, ListViewMailItemComponent item)
            {
                ListViewMailItemDescription itemData = item.itemdData;
                if (null == itemData)
                    return;

                m_mailData = itemData;
                this.UpdateUI();
            }

            ListViewMailItemDescription m_mailData;
            private void UpdateUI()
            {
                int mailCount = MailDef.mailMgr.mails.Count;
                m_pageState.SetCurrentState(mailCount > 0 ? 1 : 0, false);
                m_countPromptTxt.text = TipContentUtil.GenText("main_mail_count_prompt", mailCount, MailDef.MAIL_SAVE_MAX_COUNT);

                if (null == m_mailData || null == m_mailData.mail)
                    return;

                if (!m_mailData.mail.isRead)
                {
                    m_mailData.mail.isRead = true;
                    m_mailData.NotifyChange("isRead");
                    MailDef.mailMgr.SetReadMark(m_mailData.id);
                }

                MailDbRecord mail = m_mailData.mail;
                m_titleTxt.text = mail.title;
                m_contentTxt.text = mail.content;
                string sender = string.IsNullOrEmpty(mail.sender) ? TipContentUtil.GenText("mail_sender_default") : mail.sender;
                m_senderTxt.text = TipContentUtil.GenText("mail_sender_template", sender);
                m_attachmentState.SetCurrentState(MailDef.HasAttachement(mail) ? 1 : 0, false);
                for (int i = 0; i < m_attachmentItemContainer.childCount; ++ i)
                {
                    GameObject.Destroy(m_attachmentItemContainer.GetChild(i).gameObject);
                }
                for (int i = 0; i < mail.attachments.Count; ++ i)
                {
                    GameObject go = GameObject.Instantiate(m_attachmentItemPrefab);
                    go.transform.SetParent(m_attachmentItemContainer);
                    go.SetActive(true);
                    go.transform.localScale = Vector3.one;
                    go.transform.Find("Fetch").gameObject.SetActive(mail.isFetchedAttachments);
                    UIItemGrid uiItemGrid = go.transform.Find("Item").GetComponent<UIItemGrid>();
                    MailDbItem attachment = mail.attachments[i];
                    uiItemGrid.SetData(attachment.id, attachment.num);
                    go.GetComponent<Button>().onClick.AddListenerIfNoExist(() => { this.OnClickAttachment(attachment); });
                }
                m_fetchBtn.gameObject.SetActive(mail.attachments.Count > 0 && !mail.isFetchedAttachments);
            }

            private void OnClickAttachment(MailDbItem item)
            {
                InitItemTipsData tipsData = new InitItemTipsData();
                tipsData.type = InitItemTipsData.Type.Mail;
                tipsData.itemData = new NetProto.ItemGrid();
                tipsData.itemData.data = new ItemData();
                tipsData.itemData.data.id = item.id;
                tipsData.itemData.count = item.num;
                tipsData.index = 0;
                tipsData.m_BagType = Config.BagType.mail;
                App.my.uiSystem.ShowPanel(PanelType.UIItemTipsPanel, tipsData);
            }

            protected override void OnShow(object args)
            {
                this.ResetUI();
                Event.Subscribe<MailFetchAttachmentResult>(EventID.Mail_AttachmentFetchStateChange, OnFetchStateChange);
                Event.Subscribe(EventID.Mail_NewMailFlagChange, OnNewMailFlagChange);

                MailDef.mailMgr.newMailFlag = false;
            }
            private void OnFetchStateChange(MailFetchAttachmentResult ret)
            {
                if (0 == ret.mailId)
                    return;

                foreach (var itemData in m_listViewMailItem.DataSource)
                {
                    if (itemData.id == ret.mailId)
                    {
                        if (null != itemData.mail)
                        {
                            itemData.mail.isFetchedAttachments = ret.isFetchAttachmentVal;
                            itemData.NotifyChange("isFetchedAttachments");
                        }
                        break;
                    }
                }

                if (null != m_mailData && null != m_mailData.mail)
                {
                    if (m_mailData.id == ret.mailId)
                    {
                        this.UpdateUI();
                    }
                }
            }
            private void OnNewMailFlagChange()
            {
                if (MailDef.mailMgr.newMailFlag)
                    MailDef.mailMgr.newMailFlag = false;
            }

            private void ResetUI()
            {
                List<ListViewMailItemDescription> mailItems = new List<ListViewMailItemDescription>();
                foreach (var mail in MailDef.mailMgr.mails.Values)
                {
                    ListViewMailItemDescription desc = new ListViewMailItemDescription();
                    desc.mail = mail;
                    mailItems.Add(desc);
                }
                Comparison<ListViewMailItemDescription> comparison = new Comparison<ListViewMailItemDescription>(
                   (ListViewMailItemDescription x, ListViewMailItemDescription y) =>
                   {
                       if (x.mail.createTick != y.mail.createTick)
                           return x.mail.createTick > y.mail.createTick ? -1 : 1;
                       return x.id > y.id ? -1 : 1;
                   });
                mailItems.Sort(comparison);
                m_mailData = null;
                if (mailItems.Count > 0)
                {
                    m_mailData = mailItems[0];
                    m_mailData.isSelected = true;
                }
                m_listViewMailItem.DataSource = new ObservableList<ListViewMailItemDescription>(mailItems);
                this.UpdateUI();
            }
        }
    }

}
#endif