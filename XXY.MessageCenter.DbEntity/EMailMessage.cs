﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XXY.MessageCenter.DbEntity.Enums;

namespace XXY.MessageCenter.DbEntity {

    /// <summary>
    /// Email
    /// </summary>
    public class EMailMessage : BaseMessage {

        public EMailMessage()
            : base(MsgTypes.Email, true) {
        }

        public string Cc {
            get;
            set;
        }

        public string Bcc {
            get;
            set;
        }

        [Required, StringLength(100)]
        public string Subject {
            get;
            set;
        }
    }
}