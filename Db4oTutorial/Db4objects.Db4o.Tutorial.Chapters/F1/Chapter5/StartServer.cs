using System;
using System.Threading;
using Db4objects.Db4o;
using Db4objects.Db4o.Messaging;

namespace Db4objects.Db4o.Tutorial.F1.Chapter5
{
    /// <summary>
    /// starts a db4o server with the settings from ServerConfiguration. 
    /// This is a typical setup for a long running server.
    /// The Server may be stopped from a remote location by running
    /// StopServer. The StartServer instance is used as a MessageRecipient 
    /// and reacts to receiving an instance of a StopServer object.
    /// Note that all user classes need to be present on the server side
    /// and that all possible Db4oFactory.Configure() calls to alter the db4o
    /// configuration need to be executed on the client and on the server.
    /// </summary>
    public class StartServer : ServerConfiguration, IMessageRecipient
    {
        /// <summary>
        /// setting the value to true denotes that the server should be closed
        /// </summary>
        private bool stop = false;

        /// <summary>
        /// starts a db4o server using the configuration from
        /// ServerConfiguration.
        /// </summary>
        public static void Main(string[] arguments)
        {
            new StartServer().RunServer();
        } 

        /// <summary>
        /// opens the IObjectServer, and waits forever until Close() is called
        /// or a StopServer message is being received.
        /// </summary>
        public void RunServer()
        {
            lock(this)
            {
                IObjectServer db4oServer = Db4oFactory.OpenServer(FILE, PORT);
                db4oServer.GrantAccess(USER, PASS);
                
                // Using the messaging functionality to redirect all
                // messages to this.processMessage
                db4oServer.Ext().Configure().SetMessageRecipient(this);
                try
                {
                    if (! stop)
                    {
                        // wait forever for Notify() from Close()
                        Monitor.Wait(this);   
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                db4oServer.Close();
            }
        }

        /// <summary>
        /// messaging callback
        /// see com.db4o.messaging.MessageRecipient#ProcessMessage()
        /// </summary>
        public void ProcessMessage(IObjectContainer con, object message)
        {
            if (message is StopServer)
            {
                Close();
            }
        }

        /// <summary>
        /// closes this server.
        /// </summary>
        public void Close()
        {
            lock(this)
            {
                stop = true;
                Monitor.PulseAll(this);
            }
        }
    }
}