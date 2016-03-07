namespace UnityToolkit.Messaging
{
    public struct SimpleMessage<T>
    {
        public T Content;

        
        public SimpleMessage(T content)
        {
            Content = content;
        }
    }
}