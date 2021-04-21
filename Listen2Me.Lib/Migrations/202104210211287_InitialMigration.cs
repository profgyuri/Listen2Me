namespace Listen2Me.Lib.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Songs",
                c => new
                    {
                        SongId = c.Int(nullable: false, identity: true),
                        Artist = c.String(maxLength: 200),
                        Title = c.String(nullable: false, maxLength: 200),
                        Genre = c.String(maxLength: 50),
                        BPM = c.String(maxLength: 4),
                        Bitrate = c.String(maxLength: 4),
                        Length = c.Time(nullable: false, precision: 1),
                        Path = c.String(nullable: false, maxLength: 500),
                    })
                .PrimaryKey(t => t.SongId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Songs");
        }
    }
}
