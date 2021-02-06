namespace Listen2Me.Lib
{
    using Autofac;

    public class IOCContainer
    {
        public static IContainer Configure()
        {
            ContainerBuilder builder = new ContainerBuilder();

            return builder.Build();
        }
    }
}
