using System.Threading.Tasks;

namespace SecretsSharingTool.Api.Queue
{
    public interface IBackgroundQueueService
    {
        /// <summary>
        /// Signal the queuing service to start the background thread that would handle the queue.
        /// </summary>
        Task Start();

        /// <summary>
        /// Signal the queuing service that it should terminate as soon as possible. This should be called when
        /// the application is shutting down.
        /// </summary>
        void Stop();

        /// <summary>
        /// Signal the service to process pending items.
        /// </summary>
        void Process();
    }
}