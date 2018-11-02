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
    public class NetworkIO : BaseConsoleIO
    {
        public string ip = "127.0.0.1";
        public int port = 6001;

        private string _input = string.Empty;
        private string _output = string.Empty;
        private Queue<string> _queuedCommands = new Queue<string>();

        private void Awake()
        {
            _writer = new NetworkWriter();
        }

        void Start()
        {
            var serverThread = new Thread(new ThreadStart(StartServer));
            serverThread.IsBackground = true;
            serverThread.Start();
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
                Debug.Log(e.Message);
            }
            finally
            {
                if (listener != null)
                {
                    listener.Stop();
                    Debug.Log("Server stopped.");
                }
            }
        }

        private void HandleClients(out TcpListener listener)
        {
            var localAddress = IPAddress.Parse(ip);
            listener = new TcpListener(localAddress, port);

            listener.Start();
            Debug.Log("Server started.");

            while (true)
            {
                Debug.Log("Listening for incoming connections...");
                var client = listener.AcceptTcpClient();

                Debug.Log("Client connection established.");
                var writer = new StreamWriter(client.GetStream());
                var reader = new StreamReader(client.GetStream());
                //Write existing output to client
                WriteOutputToClient(writer);

                var clientMessage = string.Empty;
                while ((clientMessage = reader.ReadLine()) != null && clientMessage != "exit")
                {
                    Debug.Log("<-" + clientMessage);

                    EnqueueCommand(clientMessage);
                    WaitForExecution();

                    WriteOutputToClient(writer);
                }

                Debug.Log("Client connection has been lost.");
                writer.Close();
                reader.Close();
                client.Close();
            }
        }

        public override string Input
        {
            get { return _input; }
            set { _input = value; }
        }

        private void WriteOutputToClient(StreamWriter writer)
        {
            var splitStrings = _output.Split(new char[] { '\n' });

            foreach (string s in splitStrings)
            {
                //Don't write empty line or the command
                if (s == string.Empty || s.StartsWith("> ")) continue;

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

        public override void AppendToOutput(string text)
        {
            lock (_output)
            {
                _output += text;
            }
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
    }
}