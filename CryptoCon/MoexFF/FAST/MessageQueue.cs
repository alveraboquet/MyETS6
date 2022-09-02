namespace Moex.FAST
{
    internal class MessageQueue
    {
        public MessageQueue()
        {
            this.head = new QueueItem();
            this.tail = this.head;
        }

        public DataConnector.MessageInfo Enqueue(byte[] data)
        {
            QueueItem tail = this.tail;
            tail.Next = new QueueItem();
            this.tail = tail.Next;
            tail.Message.SetData(data);
            return tail.Message;
        }

        public DataConnector.MessageInfo Peek()
        {
            return this.head.Message;
        }

        public DataConnector.MessageInfo Dequeue()
        {
            QueueItem head = this.head;
            if (head.Next != null)
            {
                this.head = head.Next;
            }
            else
            {
                // ???
                this.head = new QueueItem();
                this.tail = this.head;
            }
            return head.Message;
        }

        internal class QueueItem
        {
            public QueueItem()
            {
                this.Message = new DataConnector.MessageInfo();
                this.Next = null;
            }

            public DataConnector.MessageInfo Message { get; private set; }

            public QueueItem? Next { get; set; }
        }

        public int GetCount()
        {
            int count = 0;
            QueueItem? item = this.head;
            while (item != null)
            {
                count++;
                item = item.Next;
            }
            return count;
        }

        private QueueItem head;
        private QueueItem tail;
    }
}
