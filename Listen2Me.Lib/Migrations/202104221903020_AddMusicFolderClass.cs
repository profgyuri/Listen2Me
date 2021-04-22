namespace Listen2Me.Lib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMusicFolderClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MusicFolders",
                c => new
                    {
                        MusicFolderId = c.Int(nullable: false, identity: true),
                        Path = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.MusicFolderId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MusicFolders");
        }
    }
}
