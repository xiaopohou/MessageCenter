﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace XXY.MessageCenter.Queue {
    public class QueueHolder {

        public string QueuePath {
            get;
            private set;
        }

        private IEnumerable<Type> SupportDataTypes {
            get;
            set;
        }


        public event EventHandler<DataReceivedArgs> OnDataReceived = null;

        public QueueHolder(string path, IEnumerable<Type> supportDataTypes) {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");
            if (supportDataTypes == null || supportDataTypes.Count() == 0)
                throw new ArgumentException("supportTypes");



            this.QueuePath = path;
            this.SupportDataTypes = supportDataTypes;
        }

        private MessageQueue GetQueue() {
            //远程队列无法确定是不是存在。
            //"无法确定具有指定格式名的队列是否存在,
            //只能保证这个队列一定存在。
            var queue = new MessageQueue(this.QueuePath);
            return queue;
        }


        private MessagePriority ConvertPriority(Priorities pri) {
            switch (pri) {
                case Priorities.Higher:
                    return MessagePriority.High;
                case Priorities.Immediately:
                    return MessagePriority.Highest;
                case Priorities.Lower:
                    return MessagePriority.Low;
                default:
                    return MessagePriority.Normal;
            }
        }


        public bool Put(object data, Priorities pri = Priorities.Normal) {
            if (!this.SupportDataTypes.Contains(data.GetType())) {
                throw new ArgumentException("Not Support data type");
            }


            using (var trans = new MessageQueueTransaction())
            using (var mq = this.GetQueue()) {
                trans.Begin();
                try {
                    //mq.Formatter = new JsonMessageFormater(data.GetType());
                    //mq.Formatter = new ProtoBufFormatter(data.GetType());
                    var msg = new Message(data);
                    msg.Formatter = new JsonMessageFormater(data.GetType());
                    msg.Label = data.GetType().FullName;
                    msg.Priority = this.ConvertPriority(pri);
                    msg.Recoverable = true;
                    
                    mq.Send(msg, trans);
                    trans.Commit();
                    return true;
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    trans.Abort();
                    return false;
                }
            }
        }

        public void Listen() {
            var mq = this.GetQueue();
            mq.PeekCompleted += mq_PeekCompleted;
            mq.BeginPeek();
        }

        void mq_PeekCompleted(object sender, PeekCompletedEventArgs e) {
            var queue = (MessageQueue)sender;

            var type = this.SupportDataTypes.FirstOrDefault(t => t.FullName.Equals(e.Message.Label));
            if (type != null) {
                queue.Formatter = new JsonMessageFormater(type);
                //queue.Formatter = new ProtoBufFormatter(typeof(T));
                using (var transaction = new TransactionScope()) {
                    var msg = queue.EndPeek(e.AsyncResult);
                    if (this.OnDataReceived != null)
                        this.OnDataReceived(null, new DataReceivedArgs(msg.Body));

                    queue.ReceiveById(e.Message.Id, MessageQueueTransactionType.Automatic);
                    transaction.Complete();
                }
            }
            queue.BeginPeek();
        }
    }

    public class DataReceivedArgs : EventArgs {
        public object Data {
            get;
            private set;
        }
        public DataReceivedArgs(object mail) {
            this.Data = mail;
        }
    }
}
