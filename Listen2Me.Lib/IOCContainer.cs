namespace Listen2Me.Lib
{
    using Autofac;
    using Listen2Me.Lib.Models;

    public class IOCContainer
    {
        public static IContainer Configure()
        {
            ContainerBuilder builder = new();

            builder.RegisterType<MusicPlayer>().As<IMusicPlayer>();

            return builder.Build();
        }
    }
}
