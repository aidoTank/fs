//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.UI;

//namespace Roma {
//    public class UIComChat : UI
//    {
//        //初始化部分
//        public UIChatItem chatItem;
//        public ScrollRect scrollRect;
//        public RectTransform itemParent;
//        public Text txtContent;
//        public Text txtVoice;
//        public Image imgEmotion;
//        public GameObject leftBtn;          //显示剩余数量
//        //个性化设置
//        public eChatArea chatType = eChatArea.Chat;
//        public int maxCount = 10;            //最多创建多少个            
//        public Vector2 chatRect;                //聊天窗口大小（遮罩的尺寸）                         
//        public string defaultColor = "000000";

//        //显示部分
//        private int m_startIndex = 0;              //从第几条消息开始显示
//        public ChatItemSize chatSize;
//        public ChatItemSize noticeSize;
//        private UIRichTextItemNew chatRichItem;     //聊天的，即玩家发出的（有头像，名字之类的）
//        private UIRichTextItemNew noticeRichItem;           //系统提示
//        private List<UIChatItem> chatItemList = new List<UIChatItem>();
//        private ChannelContent m_channel;        //该频道的全部内容
//        private bool m_isNew = false;       //是否新的频道，是的话全部重刷，如果不是就保留原有已经创建好的聊天内容

//        public class ChatItemSize
//        {
//            public ChatItemSize(int minHeigh, int textHeigh, int heighOffset, int contentWidth, int offset)
//            {
//                this.minHeigh = minHeigh;
//                this.textHeigh = textHeigh;
//                this.heighOffset = heighOffset;
//                this.contentWidth = contentWidth;
//                this.offset = offset;
//            }
//            public int minHeigh;        //一块聊天块的最少高度
//            public int textHeigh;       //文字内容的高度
//            public int heighOffset;     //行间距
//            public int contentWidth;        //聊天内容的宽度
//            public int offset;                  //距离调整
//        }


//        public override void Init()
//        {
//            base.Init();
//            chatItem = GetChild("scroll/top/item").gameObject.AddComponent<UIChatItem>();
//            scrollRect = GetChild("scroll").gameObject.GetComponent<ScrollRect>();
//            scrollRect.onValueChanged.AddListener(delegate { OnValueChange(); });
//            chatRect = scrollRect.GetComponent<RectTransform>().sizeDelta;
//            itemParent = GetChild("scroll/top/item_parent").GetComponent<RectTransform>();
//            txtContent = GetChild("content").GetComponent<Text>();
//            txtVoice = GetChild("voice").GetComponent<Text>();
//            imgEmotion = GetChild("emotion").GetComponent<Image>();
//            chatRichItem = new UIRichTextItemNew(txtContent, imgEmotion, chatSize.textHeigh, chatSize.heighOffset, chatSize.contentWidth, defaultColor);
//            noticeRichItem = new UIRichTextItemNew(txtContent, imgEmotion, noticeSize.textHeigh, chatSize.heighOffset, noticeSize.contentWidth, defaultColor);
//        }

//        public void InitItem()
//        {
//            for (int i = 0; i < maxCount; i++)
//            {
//                UIChatItem item = Instantiate(chatItem) as UIChatItem;
//                item.InitChatItem(chatSize.contentWidth, chatSize.textHeigh, chatSize.heighOffset, defaultColor);
//                item.InitNoticetem(noticeSize.contentWidth, noticeSize.textHeigh, chatSize.heighOffset, defaultColor);
//                item.chatArea = chatType;
//                item.Init();
//                item.transform.SetParent(itemParent);
//                item.transform.localPosition = Vector3.zero;
//                item.transform.localScale = Vector3.one;
//                item.gameObject.SetActive(false);
//                chatItemList.Add(item);
//            }
//        }

//        public void ChangeColor(string color)
//        {
//            defaultColor = color;
//            for (int i = 0; i < chatItemList.Count; i++)
//            {
//                chatItemList[i].ChangeColor(defaultColor);
//            }
//        }


//        //初始化一个聊天内容（主要是用于保存内容的长度，宽度之类的信息）
//        public void InitContent(ref ChatContent chatContent)
//        {
//            if (chatContent.type == eChatContentType.Normal || chatContent.type == eChatContentType.Horn || chatContent.type == eChatContentType.Team || chatContent.type == eChatContentType.Tips)
//            {
//                if (chatContent.isPlayer)
//                {
//                    chatContent.contentHeigh = chatRichItem.SplitText(chatContent.text);
//                    chatContent.contentWidth = chatRichItem.GetWidth();
//                    chatContent.itemHeigh = Mathf.Max(chatContent.contentHeigh + chatSize.offset, chatSize.minHeigh);
//                }
//                else
//                {
//                    chatContent.contentHeigh = noticeRichItem.SplitText(chatContent.text);
//                    chatContent.contentWidth = noticeRichItem.GetWidth();
//                    chatContent.itemHeigh = Mathf.Max(chatContent.contentHeigh + noticeSize.offset, noticeSize.minHeigh);
//                }
//            }
//            else if (chatContent.type == eChatContentType.Voice)
//            {
//                txtVoice.text = chatContent.voiceInfo.text;
//                chatContent.contentHeigh = (int)txtVoice.preferredHeight;
//                chatContent.contentWidth = (int)txtVoice.preferredWidth;
//                chatContent.itemHeigh = 100 + chatContent.contentHeigh;
//            }
//        }
//        //显示聊天的内容
//        public void CreateContent(ChannelContent channel)
//        {
//            if (m_channel == null)
//                m_isNew = true;
//            else m_isNew = (m_channel.channel != channel.channel);
//            m_channel = channel;
//            itemParent.sizeDelta = new Vector2(0, m_channel.heigh);
//            MoveToBottom();
//        }
//        public void UpdateContentHeigh(int heigh)
//        {
//            //m_contentHeigh = heigh;
//            itemParent.sizeDelta = new Vector2(0, heigh);
//        }
//        //插入一条消息
//        public void AddContent(ChatContent content)
//        {
//            itemParent.sizeDelta = new Vector2(0, m_channel.heigh);
//            //如果是玩家自己发的或者已经在底部，就直接新消息跳到底部，别人发的就看情况确定
//            if (content.isMaster || m_channel.heigh - chatRect.y <= itemParent.localPosition.y + content.itemHeigh)
//                MoveToBottom();
//            else if (m_channel.heigh <= chatRect.y)
//                curShowNum = m_channel.GetNum() - 1;
//            else if (m_channel.GetContetnList().Count >= ChatMgr.m_saveCount && GetStartIndex((int)itemParent.localPosition.y) == 0)      //如果超出了，需要重新刷（后面的条件判断m_startIndex为0为试用）
//                UpdateChatContent();
//            else
//                UpdateLeftBtn();
//        }

//        //窗口滑动
//        public void OnValueChange()
//        {
//            //------计算从第几个开始显示-------//
//            int heigh = (int)itemParent.localPosition.y;
//            //已经到达顶部或者底部
//            if (m_channel == null || heigh < 0 || heigh > m_channel.heigh - chatRect.y)
//                return;
//            m_startIndex = GetStartIndex(heigh);
//            UpdateChatContent();
//            UpdateLeftCount();
//        }
//        //当前移动到某个高度后，获取初始的内容序号
//        public int GetStartIndex(int y)
//        {
//            if (m_channel.GetContetnList().Count <= maxCount)
//                return 0;
//            for (int i = 0; i < m_channel.GetContetnList().Count; i++)
//            {
//                if (-m_channel.GetContetnList()[i].yPos <= y && -m_channel.GetContetnList()[i].yPos + m_channel.GetContetnList()[i].itemHeigh > y)
//                {
//                    if (i + maxCount > m_channel.GetContetnList().Count)
//                        return m_channel.GetContetnList().Count - maxCount;
//                    else return i;
//                }
//            }
//            return 0;
//        }



//        //从起始序号到结束序号的显示出来
//        Dictionary<int, int> recordNumList = new Dictionary<int, int>();       //记录需要显示的序号和编号
//        List<UIChatItem> recordItemList = new List<UIChatItem>();            //记录需要重新刷的消息块
//        Dictionary<ulong, UIChatItem> m_dicChatItem = new Dictionary<ulong, UIChatItem>();
//        public void UpdateChatContent()
//        {
//            if (m_channel.GetContetnList().Count <= maxCount)
//            {
//                for (int i = 0; i < m_channel.GetContetnList().Count; i++)
//                {
//                    if (m_dicChatItem.ContainsKey(chatItemList[i].uid))
//                        m_dicChatItem.Remove(chatItemList[i].uid);
//                    m_dicChatItem[m_channel.GetContetnList()[i].uid] = chatItemList[i];
//                    chatItemList[i].SetChatContent(m_channel.GetContetnList()[i]);
//                }
//                ShowListNum(itemParent, m_channel.GetContetnList().Count);
//                return;
//            }
//            recordNumList.Clear();
//            recordItemList.Clear();
//            for (int i = 0; i < maxCount; i++)
//            {
//                recordNumList[m_channel.GetContent(m_startIndex + i).num] = m_startIndex + i;
//            }
//            for (int i = 0; i < chatItemList.Count; i++)
//            {
//                //检测是不是原有的，原有的不动
//                if (recordNumList.ContainsKey(chatItemList[i].num) && !m_isNew)
//                {
//                    recordNumList.Remove(chatItemList[i].num);
//                    if (m_channel.GetContetnList().Count >= ChatMgr.m_saveCount)        //如果是已经超出最大数量，不需要刷新内容，但要刷新位置
//                        chatItemList[i].UpdatePosition();
//                    //语音刷新一下吧
//                    if (chatItemList[i].chatType == eChatContentType.Voice)
//                        chatItemList[i].CheckVoiceRefresh();
//                }
//                else
//                {
//                    //新的有变化的需要重新刷
//                    recordItemList.Add(chatItemList[i]);
//                }
//            }
//            var e = recordNumList.GetEnumerator();
//            int itemIndex = 0;
//            while (e.MoveNext())
//            {
//                int index = e.Current.Value;
//                if (itemIndex < recordItemList.Count)
//                {
//                    if (m_dicChatItem.ContainsKey(recordItemList[itemIndex].uid))
//                        m_dicChatItem.Remove(recordItemList[itemIndex].uid);
//                    m_dicChatItem[m_channel.GetContetnList()[index].uid] = recordItemList[itemIndex];
//                    recordItemList[itemIndex].SetChatContent(m_channel.GetContetnList()[index]);
//                    itemIndex++;
//                }
//            }
//            ShowListNum(itemParent, maxCount);
//        }
//        //显示最顶
//        public void MoveToTop()
//        {
//            itemParent.transform.localPosition = Vector3.zero;
//            m_startIndex = GetStartIndex(0);
//            UpdateChatContent();
//            scrollRect.verticalNormalizedPosition = 1;
//        }
//        //显示最底
//        public void MoveToBottom()
//        {
//            int heigh = (int)Mathf.Max(m_channel.heigh - chatRect.y, 0);
//            m_startIndex = GetStartIndex(heigh);
//            UpdateChatContent();
//            curShowNum = m_channel.GetNum() - 1;
//            scrollRect.verticalNormalizedPosition = 0;
//        }

//        public void UpdateLeftCount()
//        {
//            if (leftBtn == null || curShowNum == m_channel.GetNum() - 1)
//                return;
//            int heigh = (int)(itemParent.localPosition.y + chatRect.y);
//            int endIndex = m_channel.GetContetnList().Count - 1;               //屏幕看得到的最后一条消息
//            for (int i = m_startIndex; i < m_channel.GetContetnList().Count; i++)
//            {
//                if (-m_channel.GetContetnList()[i].yPos <= heigh && -m_channel.GetContetnList()[i].yPos + m_channel.GetContetnList()[i].itemHeigh > heigh)
//                {
//                    endIndex = i;
//                    break;
//                }
//            }
//            if (m_channel.GetContent(endIndex).num > curShowNum)
//                curShowNum = m_channel.GetContent(endIndex).num;
//        }

//        int m_curShowIndex = 0;
//        public int curShowNum
//        {
//            get
//            {
//                return m_curShowIndex;
//            }
//            set
//            {
//                if (m_curShowIndex == value)
//                    return;
//                m_curShowIndex = value;
//                UpdateLeftBtn();
//            }
//        }

//        private void UpdateLeftBtn()
//        {
//            int left = m_channel.GetNum() - curShowNum - 1;
//            if (left > 0)
//            {
//                leftBtn.SetActive(true);
//                UIButton.Get(leftBtn).SetText("当前消息" + left + "条，点击自动显示最新内容");
//            }
//            else
//            {
//                leftBtn.SetActive(false);
//            }
//        }



//        //显示子物体的数量
//        public void ShowListNum(Transform trans, int num)
//        {
//            if (trans.childCount < num)
//                return;
//            for (int i = 0; i < num; i++)
//            {
//                trans.GetChild(i).gameObject.SetActive(true);
//            }
//            for (int i = num; i < trans.childCount; i++)
//            {
//                trans.GetChild(i).gameObject.SetActive(false);
//            }
//        }

//        public UIChatItem GetChatItem(ulong uid)
//        {
//            if (m_dicChatItem.ContainsKey(uid))
//                return m_dicChatItem[uid];
//            return null;
//        }
//    }
//}
