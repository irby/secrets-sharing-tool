using System;
using System.Threading;
using System.Threading.Tasks;

namespace SecretsSharingTool.Api.Queue
{
    public abstract class BackgroundThread
    {
        private readonly object _locker = new object();
        private bool _initialized;
        private Thread? _thread;

        protected BackgroundThread(string name)
        {
            Name = name;
            Exit = false;
        }

        public void Start()
        {
            lock (_locker)
            {
                if (_initialized) throw new Exception("You can only initialize once, please only call Start() once.");

                _thread = new Thread(Work)
                {
                    Name = Name,
                    IsBackground = false
                };
                _thread.Start();

                _initialized = true;
            }
        }

        protected void Work()
        {
            try
            {
                while (true)
                {
                    lock (_locker)
                    {
                        // wait for items to queue up
                        while (Requests == 0)
                        {
                            if (AutomaticWakeup != null)
                            {
                                // Sleep until a pulse is called or until 'AutomaticWakeup' milliseconds have passed.
                                int sleepForMs = AutomaticWakeup.Value;
                                //Log($"{Name} waiting for {sleepForMs}.");
                                Monitor.Wait(_locker, sleepForMs);
                                //Log($"{Name} done waiting.");
                                // Make sure we have at least one request so that we would exit the loop.
                                Requests = Math.Max(Requests, 1);
                            }
                            else
                            {
                                // Sleep until a pulse is called.
                                Monitor.Wait(_locker);
                            }

                            // if we've been signaled to exit we should do so immediately
                            if (Exit)
                            {
                                return;
                            }
                        }

                        Requests--;
                        // if we've been signaled to exit we should do so immediately
                        if (Exit) return;
                    }

                    try
                    {
                        ProcessNextBatch();
                    }
                    catch (Exception ex)
                    {
                        Log(ex);
                    }

                    // if we've been signaled to exit we should do so immediately
                    if (Exit) return;
                }
            }
            catch (ThreadAbortException)
            {
                // ThreadAbortException are expected when IIS shuts down the application pool
                throw;
            }
            catch (Exception ex)
            {
                try
                {
                    Log(ex);
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch
                    // ReSharper restore EmptyGeneralCatchClause
                {
                    // we can't let any exceptions propagate any further as that would crash the thread
                }
            }
        }

        private void ProcessNextBatch()
        {
            if (!Exit)
            {
                //Log($"Running task for {Name}.");
                Task.Run(async () => { await RunTask(); }).Wait();
                //Log($"Task successfully ran for {Name}.");
            }
        }

        private async Task RunTask()
        {
            try
            {
                await WorkerTask();
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        protected abstract Task WorkerTask();
        protected abstract void Log(Exception ex);
        protected abstract void Log(string message);

        /// <summary>
        /// Number of milliseconds the current queue should wake up and start processing. If not set then the current
        /// thread does not wake up and always waits for signal to process.
        /// </summary>
        protected int? AutomaticWakeup { get; set; }

        /// <summary>
        /// Put the current queue on pause for a number of milliseconds.
        /// </summary>
        protected void Throttle(int milliseconds)
        {
            if (milliseconds > 0)
            {
                lock (_locker)
                {
                    Monitor.Wait(_locker, milliseconds);
                }
            }
        }

        /// <summary>
        /// Holds a counter to the number of requests from the main application. Every time items
        /// have been added to the processing queue the number of requests should be incremented.
        /// </summary>
        private int Requests { get; set; }

        /// <summary>
        /// Increments the number of requests that need to be process and wakes up the thread for
        /// processing.
        /// </summary>
        public void Process()
        {
            lock (_locker)
            {
                if (!_initialized)
                    throw new Exception("The queue has not been initialized, please call Start() before Process().");

                if (!Exit)
                {
                    Requests++;
                    Monitor.PulseAll(_locker);
                    Log($"{Name} queued for processing.");
                }
            }
        }

        /// <summary>
        /// If true, the current thread should terminate as soon as possible
        /// </summary>
        protected bool Exit { get; private set; }

        /// <summary>
        /// Name that identifies this thread
        /// </summary>
        protected string Name { get; private set; }

        /// <summary>
        /// Signals the current thread that it should terminate as soon as possible
        /// </summary>
        public void Stop()
        {
            lock (_locker)
            {
                Exit = true;
                Log($"Requesting to stop {Name}...");
                Monitor.PulseAll(_locker);
            }

            Log($"Pulsed locker {Name}. Joining threads...");
            _thread?.Join();
            Log($"Joined threads {Name}.");
        }

        /// <summary>
        /// Signal sent by the environment that the application pool is recycling.
        /// </summary>
        public void Stop(bool immediate)
        {
            if (!immediate)
            {
                Log($"Request to stop: {Name}");
                Stop();
            }
        }
    }
}