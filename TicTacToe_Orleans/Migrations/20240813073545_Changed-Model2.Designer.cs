﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TicTacToe_Orleans_.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240813073545_Changed-Model2")]
    partial class ChangedModel2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TicTacToe_Orleans.Model.GameRoom", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Draw")
                        .HasColumnType("integer");

                    b.Property<string>("O")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("OWins")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<string>("X")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("XWins")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("GameRooms");
                });

            modelBuilder.Entity("TicTacToe_Orleans.Model.Invitation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Accept")
                        .HasColumnType("boolean");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("GameRoom")
                        .HasColumnType("uuid");

                    b.Property<bool>("NewInvite")
                        .HasColumnType("boolean");

                    b.Property<string>("To")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Invitations");
                });

            modelBuilder.Entity("TicTacToe_Orleans.Model.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
