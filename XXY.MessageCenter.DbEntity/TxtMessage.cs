﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XXY.MessageCenter.DbEntity {

    /// <summary>
    /// 文本消息
    /// </summary>
    public class TxtMessage : BaseMessage {

        public TxtMessage()
            : base(Enums.MsgTypes.Txt, true) {
        }


        [Required, StringLength(100)]
        public string Subject {
            get;
            set;
        }

        public bool Readed {
            get;
            set;
        }

        [Required, StringLength(20)]
        public string Sender {
            get;
            set;
        }

        public decimal SenderID {
            get;
            set;
        }

        public decimal ReceiverID {
            get;
            set;
        }
    }
}