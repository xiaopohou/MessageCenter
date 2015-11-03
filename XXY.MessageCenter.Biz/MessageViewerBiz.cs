﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XXY.Common.Attributes;
using XXY.MessageCenter.BizEntity.Conditions;
using XXY.MessageCenter.DbContext;
using XXY.MessageCenter.DbEntity;
using XXY.MessageCenter.DbEntity.Enums;
using XXY.MessageCenter.IBiz;

namespace XXY.MessageCenter.Biz {

    [AutoInjection(typeof(IMessageViewer))]
    public class MessageViewerBiz : BaseBiz, IMessageViewer {
        public async Task<IEnumerable<BaseMessage>> Search(MessageSearchCondition cond) {
            var handler = MessageHandlerFactory.GetHandler(cond.MsgType);
            return await handler.Search(cond);
        }


        public async Task<BaseMessage> Get(MsgTypes type, int id) {
            var handler = MessageHandlerFactory.GetHandler(type);
            return await handler.Get(id);
        }


        public async Task<bool> Delete(MsgTypes type, int id) {
            var handler = MessageHandlerFactory.GetHandler(type);
            return await handler.Delete(id);
        }


        public async Task<TxtMessage> GetTxtMsg(int msgID, double receiverID) {
            using (var db = new Entities()) {
                return await db.TxtMessages.FirstOrDefaultAsync(t => !t.IsDeleted && t.ReceiverID == receiverID && t.ID == msgID);
            }
        }


        public async Task<int> GetUnReadTxtMsgCount(double receiverID) {
            using (var db = new Entities()) {
                return await db.TxtMessages.CountAsync(t => t.ReceiverID == receiverID && !t.IsDeleted && !t.Readed);
            }
        }


        public async Task<bool> SetTxtMsgReaded(int msgID) {
            using (var db = new Entities()) {
                var c = await db.TxtMessages.FirstOrDefaultAsync(t => t.ID == msgID && !t.IsDeleted && !t.Readed);
                if (c != null) {
                    c.Readed = true;
                    this.SetModifyInfo(c);
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }
    }
}