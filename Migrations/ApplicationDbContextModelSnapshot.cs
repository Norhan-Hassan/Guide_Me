﻿// <auto-generated />
using Guide_Me.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Guide_Me.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Guide_Me.Models.City", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CityImage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CityName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("Guide_Me.Models.Place", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CityId")
                        .HasColumnType("int");

                    b.Property<string>("PlaceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.ToTable("Places");
                });

            modelBuilder.Entity("Guide_Me.Models.PlaceItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int>("placeID")
                        .HasColumnType("int");

                    b.Property<string>("placeItemName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("placeID");

                    b.ToTable("placeItem");
                });

            modelBuilder.Entity("Guide_Me.Models.PlaceItemMedia", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("MediaContent")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MediaType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("placeItemID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("placeItemID");

                    b.ToTable("placeItemMedias");
                });

            modelBuilder.Entity("Guide_Me.Models.PlaceMedia", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("MediaContent")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MediaType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PlaceId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PlaceId");

                    b.ToTable("placeMedias");
                });

            modelBuilder.Entity("Guide_Me.Models.Place", b =>
                {
                    b.HasOne("Guide_Me.Models.City", "City")
                        .WithMany("Places")
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("City");
                });

            modelBuilder.Entity("Guide_Me.Models.PlaceItem", b =>
                {
                    b.HasOne("Guide_Me.Models.Place", "place")
                        .WithMany("PlaceItems")
                        .HasForeignKey("placeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("place");
                });

            modelBuilder.Entity("Guide_Me.Models.PlaceItemMedia", b =>
                {
                    b.HasOne("Guide_Me.Models.PlaceItem", "placeItem")
                        .WithMany("PlaceItemMedias")
                        .HasForeignKey("placeItemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("placeItem");
                });

            modelBuilder.Entity("Guide_Me.Models.PlaceMedia", b =>
                {
                    b.HasOne("Guide_Me.Models.Place", "Place")
                        .WithMany("PlaceMedias")
                        .HasForeignKey("PlaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Place");
                });

            modelBuilder.Entity("Guide_Me.Models.City", b =>
                {
                    b.Navigation("Places");
                });

            modelBuilder.Entity("Guide_Me.Models.Place", b =>
                {
                    b.Navigation("PlaceItems");

                    b.Navigation("PlaceMedias");
                });

            modelBuilder.Entity("Guide_Me.Models.PlaceItem", b =>
                {
                    b.Navigation("PlaceItemMedias");
                });
#pragma warning restore 612, 618
        }
    }
}
