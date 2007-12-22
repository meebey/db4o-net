/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Messaging;
using Db4objects.Db4o.Messaging.Internal;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MUserMessage : MsgObject, IServerSideMessage, IClientSideMessage
		, IMessageContextProvider
	{
		public bool ProcessAtServer()
		{
			return ProcessUserMessage();
		}

		public bool ProcessAtClient()
		{
			return ProcessUserMessage();
		}

		private bool ProcessUserMessage()
		{
			IMessageRecipient recipient = MessageRecipient();
			if (recipient == null)
			{
				return true;
			}
			try
			{
				MessageContextInfrastructure.contextProvider.With(this, new _IRunnable_26(this, recipient
					));
			}
			catch (Exception x)
			{
				Sharpen.Runtime.PrintStackTrace(x);
			}
			return true;
		}

		private sealed class _IRunnable_26 : IRunnable
		{
			public _IRunnable_26(MUserMessage _enclosing, IMessageRecipient recipient)
			{
				this._enclosing = _enclosing;
				this.recipient = recipient;
			}

			public void Run()
			{
				recipient.ProcessMessage(this._enclosing.Transaction().ObjectContainer(), this._enclosing
					.ReadUserMessage());
			}

			private readonly MUserMessage _enclosing;

			private readonly IMessageRecipient recipient;
		}

		public Db4objects.Db4o.Messaging.MessageContext MessageContext()
		{
			return new _MessageContext_41(this);
		}

		private sealed class _MessageContext_41 : Db4objects.Db4o.Messaging.MessageContext
		{
			public _MessageContext_41(MUserMessage _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private sealed class _IMessageSender_43 : IMessageSender
			{
				public _IMessageSender_43(_MessageContext_41 _enclosing)
				{
					this._enclosing = _enclosing;
				}

				public void Send(object message)
				{
					this._enclosing._enclosing.ServerMessageDispatcher().Write(Msg.USER_MESSAGE.MarshallUserMessage
						(this._enclosing._enclosing.Transaction(), message));
				}

				private readonly _MessageContext_41 _enclosing;
			}

			public override IMessageSender Sender
			{
				get
				{
					return new _IMessageSender_43(this);
				}
			}

			private readonly MUserMessage _enclosing;
		}

		private object ReadUserMessage()
		{
			Unmarshall();
			return ((MUserMessage.UserMessagePayload)ReadObjectFromPayLoad()).message;
		}

		private IMessageRecipient MessageRecipient()
		{
			return Config().MessageRecipient();
		}

		public sealed class UserMessagePayload
		{
			public object message;

			public UserMessagePayload()
			{
			}

			public UserMessagePayload(object message_)
			{
				message = message_;
			}
		}

		public Msg MarshallUserMessage(Transaction transaction, object message)
		{
			return GetWriter(Serializer.Marshall(transaction, new MUserMessage.UserMessagePayload
				(message)));
		}
	}
}
