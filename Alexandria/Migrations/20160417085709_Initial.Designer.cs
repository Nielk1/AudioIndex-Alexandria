using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using Alexandria.Models;

namespace Alexandria.Migrations
{
    [DbContext(typeof(MusicContext))]
    [Migration("20160417085709_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Alexandria.Models.File", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Archival");

                    b.Property<DateTime>("FileModified");

                    b.Property<int?>("LibraryID")
                        .IsRequired();

                    b.Property<string>("Path")
                        .IsRequired();

                    b.Property<DateTime>("RecordAdded");

                    b.HasKey("ID");
                });

            modelBuilder.Entity("Alexandria.Models.Library", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Enabled");

                    b.Property<DateTime?>("Expire");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Path")
                        .IsRequired();

                    b.HasKey("ID");
                });

            modelBuilder.Entity("Alexandria.Models.Tag", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CategoryID")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ID");
                });

            modelBuilder.Entity("Alexandria.Models.TagAdder", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("TagAssociationID");

                    b.Property<DateTime>("Timestamp");

                    b.Property<int?>("UserID");

                    b.HasKey("ID");
                });

            modelBuilder.Entity("Alexandria.Models.TagAssociation", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("FileID");

                    b.Property<int?>("TagID")
                        .IsRequired();

                    b.Property<int?>("VirtualAlbumID");

                    b.Property<int?>("VirtualTrackID");

                    b.HasKey("ID");
                });

            modelBuilder.Entity("Alexandria.Models.TagCategory", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("ID");
                });

            modelBuilder.Entity("Alexandria.Models.UserAccount", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Admin");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("Password");

                    b.Property<bool>("System");

                    b.Property<string>("UserName")
                        .IsRequired();

                    b.HasKey("ID");
                });

            modelBuilder.Entity("Alexandria.Models.VirtualAlbum", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("FileID");

                    b.HasKey("ID");
                });

            modelBuilder.Entity("Alexandria.Models.VirtualTrack", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("FileID");

                    b.Property<int?>("vAlbumID");

                    b.HasKey("ID");
                });

            modelBuilder.Entity("Alexandria.Models.File", b =>
                {
                    b.HasOne("Alexandria.Models.Library")
                        .WithMany()
                        .HasForeignKey("LibraryID");
                });

            modelBuilder.Entity("Alexandria.Models.Tag", b =>
                {
                    b.HasOne("Alexandria.Models.TagCategory")
                        .WithMany()
                        .HasForeignKey("CategoryID");
                });

            modelBuilder.Entity("Alexandria.Models.TagAdder", b =>
                {
                    b.HasOne("Alexandria.Models.TagAssociation")
                        .WithMany()
                        .HasForeignKey("TagAssociationID");

                    b.HasOne("Alexandria.Models.UserAccount")
                        .WithMany()
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("Alexandria.Models.TagAssociation", b =>
                {
                    b.HasOne("Alexandria.Models.File")
                        .WithMany()
                        .HasForeignKey("FileID");

                    b.HasOne("Alexandria.Models.Tag")
                        .WithMany()
                        .HasForeignKey("TagID");

                    b.HasOne("Alexandria.Models.VirtualAlbum")
                        .WithMany()
                        .HasForeignKey("VirtualAlbumID");

                    b.HasOne("Alexandria.Models.VirtualTrack")
                        .WithMany()
                        .HasForeignKey("VirtualTrackID");
                });

            modelBuilder.Entity("Alexandria.Models.VirtualAlbum", b =>
                {
                    b.HasOne("Alexandria.Models.File")
                        .WithMany()
                        .HasForeignKey("FileID");
                });

            modelBuilder.Entity("Alexandria.Models.VirtualTrack", b =>
                {
                    b.HasOne("Alexandria.Models.File")
                        .WithMany()
                        .HasForeignKey("FileID");

                    b.HasOne("Alexandria.Models.VirtualAlbum")
                        .WithMany()
                        .HasForeignKey("vAlbumID");
                });
        }
    }
}
