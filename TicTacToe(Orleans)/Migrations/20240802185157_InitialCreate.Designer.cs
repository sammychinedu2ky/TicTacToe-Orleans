﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TicTacToe_Orleans_.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240802185157_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TicTacToe_Orleans_.Model.GamePlay", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<List<List<char>>>("Board")
                        .IsRequired()
                        .HasColumnType("json")
                        .HasColumnName("Board");

                    b.Property<List<string>>("Moves")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.Property<string>("O")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Winner")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("X")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("GamePlay");
                });

            modelBuilder.Entity("TicTacToe_Orleans_.Model.Invite", b =>
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

                    b.ToTable("Invite");
                });

            modelBuilder.Entity("TicTacToe_Orleans_.Model.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("User");
                });
#pragma warning restore 612, 618
        }
    }
}
