using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{
    public class UIPanelLogin : UIBase {
        public InputField m_id, m_passwords;  //输入名字 密码
        public Text m_inputTipText;   //登录失败提示
        public Button m_sureBtn;         //登录按钮
                                         //文字
        public Text  passWord,loginWord;


        public override void Init()
        {
            base.Init();
            SetPath();
        }
        private void SetPath()
        {
            
            m_id = m_root.FindChild("panel/dynamic/input").GetComponent<InputField>();
            m_passwords= m_root.FindChild("panel/dynamic/password/input").GetComponent<InputField>();
            m_inputTipText= m_root.FindChild("panel/dynamic/tip").GetComponent<Text>();
            m_sureBtn = m_root.FindChild("panel/dynamic/sure").GetComponent<Button>();
            passWord=m_root.FindChild("panel/dynamic/password").GetComponent<Text>();
        }

        /// <summary>
        /// 获取玩家输入名字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string GetNameText()
        {
            return m_id.text.Trim();
        }

        /// <summary>
        /// 获取玩家输入密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string GetPassWordText()
        {
            return m_passwords.text.Trim();
        }

        /// <summary>
        /// id输入提示
        /// </summary>
        public void SetIdTip(string str)
        {
            m_id.placeholder.GetComponent<Text>().text = str;
        }
        /// <summary>
        /// 设置登录失败提示
        /// </summary>
        /// <param name="text"></param>
        /// <param name="t"></param>
        public void SetText(Text text, string t)
        {
            text.text = t;
        }

    }
}

