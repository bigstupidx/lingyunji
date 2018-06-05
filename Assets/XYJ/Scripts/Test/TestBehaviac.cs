using behaviac;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys;

public class TestBehaviac : MonoBehaviour 
{
    public string aiid;
    BT_RoleAgent m_agent;
    
	// Use this for initialization
	void Start () 
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (m_agent == null)
                m_agent = new BT_RoleAgent(App.my.localPlayer);

            m_agent.LoadAI(aiid);
        }

        if (m_agent != null)
            m_agent.Update();
	}
}
