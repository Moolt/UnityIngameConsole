using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace IngameConsole
{
    [RequireComponent(typeof(ConsoleLogic))]
    public class NetworkIO : BaseConsoleIO<NetworkWriter>
    {
        public string ip = "127.0.0.1";
        public int port = 6001;
        public bool logDebugInfo = false;

        private string _input = string.Empty;
        private string _output = string.Empty;
        private Queue<string> _queuedCommands = new Queue<string>();
        private string welcomeMessage;
        private TcpClient _client;
        private TcpListener _listener;
        private bool _applicationExited = false;

        private void Start()
        {
            var serverThread = new Thread(new ThreadStart(StartServer));
            serverThread.IsBackground = true;
            serverThread.Start();
            welcomeMessage = _output;
            _output = string.Empty;
        }

        private void Update()
        {
            if (HasQueuedCommands)
            {
                var executionLock = new object();
                lock (executionLock)
                {
                    _input = _queuedCommands.Dequeue();
                    RaiseInputReceived();
                }
            }
        }

        public override string Input
        {
            get { return _input; }
            set { _input = value; }
        }

        public override void AppendToOutput(string text)
        {
            lock (_output)
            {
                _output += text;
            }
        }

        private void WriteOutputToClient(StreamWriter writer)
        {
            var splitStrings = _output.Split(new char[] { '\n' });

            foreach (string s in splitStrings)
            {
                //Don't write empty line or the command
                if (s == string.Empty || s.StartsWith("> ")) continue;

                ConditionalLog("-> " + s);
                writer.WriteLine(s);
                writer.Flush();
            }

            writer.Write('\0');
            writer.Flush();

            lock (_output)
            {
                _output = string.Empty;
            }
        }

        private bool HasQueuedCommands
        {
            get
            {
                lock (_queuedCommands)
                {
                    return _queuedCommands.Count > 0;
                }
            }
        }

        private void StartServer()
        {
            TcpListener listener = null;

            try
            {
                HandleClients(out listener);
            }
            catch (Exception e)
            {
                ConditionalLog(e.Message);
            }
            finally
            {
                if (listener != null)
                {
                    listener.Stop();
                    ConditionalLog("Server stopped.");
                }
            }
            ConditionalLog("Thread closed.");
        }

        private void HandleClients(out TcpListener listener)
        {
            var localAddress = IPAddress.Parse(ip);
            listener = new TcpListener(localAddress, port);

            listener.Start();
            ConditionalLog("Server started.");

            AppendToOutput(welcomeMessage);

            while (!_applicationExited)
            {
                ConditionalLog("Listening for incoming connections...");
                using (_client = listener.AcceptTcpClient())
                {
                    ConditionalLog("Client connection established.");
                    using (var writer = new StreamWriter(_client.GetStream()))
                    using (var reader = new StreamReader(_client.GetStream()))
                    {
                        //Write existing output to client
                        WriteOutputToClient(writer);

                        var clientMessage = string.Empty;

                        while (IsClientConnected(reader) && (clientMessage = reader.ReadLine()) != null && clientMessage != "exit")
                        {
                            ConditionalLog("<-" + clientMessage);

                            EnqueueCommand(clientMessage);
                            WaitForExecution();

                            WriteOutputToClient(writer);
                        }

                        ConditionalLog("Client connection has been lost.");
                    }
                }
            }
        }

        void OnApplicationQuit()
        {
            _applicationExited = true;

            ConditionalLog("Application quits...");

            try
            {
                lock (_client)
                {
                    _client.Close();
                }
                ConditionalLog("Force closed client.");
            }
            catch { }

            try
            {
                lock(_listener)
                {
                    _listener.Stop();
                }
                ConditionalLog("Forced server to close.");
            }
            catch { }
        }

        private void EnqueueCommand(string command)
        {
            lock (_queuedCommands)
            {
                _queuedCommands.Enqueue(command);
            }
        }

        private void WaitForExecution()
        {
            while (HasQueuedCommands) { Thread.Sleep(50); }
        }

        private void ConditionalLog(string text)
        {
            if (logDebugInfo)
            {
                Debug.Log(text);
            }
        }

        private bool IsClientConnected(StreamReader reader)
        {
            try
            {
                reader.Peek();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}