﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuizAI.Infrastructure.Persistence;

#nullable disable

namespace QuizAI.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250113180347_AddVersioningColumnsToQuizzes")]
    partial class AddVersioningColumnsToQuizzes
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CategoryQuiz", b =>
                {
                    b.Property<int>("CategoriesId")
                        .HasColumnType("int");

                    b.Property<Guid>("QuizzesId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CategoriesId", "QuizzesId");

                    b.HasIndex("QuizzesId");

                    b.ToTable("QuizCategories", (string)null);
                });

            modelBuilder.Entity("QuizAI.Domain.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("QuizAI.Domain.Entities.Image", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FileExtension")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.Property<byte[]>("Hash")
                        .IsRequired()
                        .HasColumnType("VARBINARY(40)");

                    b.HasKey("Id");

                    b.HasIndex("Hash")
                        .IsUnique();

                    b.ToTable("Images");
                });

            modelBuilder.Entity("QuizAI.Domain.Entities.MultipleChoiceAnswer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("IsCorrect")
                        .HasColumnType("bit");

                    b.Property<int>("QuestionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.ToTable("MultipleChoiceAnswers");
                });

            modelBuilder.Entity("QuizAI.Domain.Entities.OpenEndedAnswer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("QuestionId")
                        .HasColumnType("int");

                    b.Property<string>("ValidContent")
                        .IsRequired()
                        .HasMaxLength(1291)
                        .HasColumnType("nvarchar(1291)");

                    b.Property<bool>("VerificationByAI")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId")
                        .IsUnique();

                    b.ToTable("OpenEndedAnswers");
                });

            modelBuilder.Entity("QuizAI.Domain.Entities.Question", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<Guid?>("ImageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte>("Order")
                        .HasColumnType("tinyint");

                    b.Property<Guid>("QuizId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ImageId");

                    b.HasIndex("QuizId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("QuizAI.Domain.Entities.Quiz", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<string>("Description")
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<Guid?>("ImageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeprecated")
                        .HasColumnType("bit");

                    b.Property<Guid?>("LatestVersionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("Id");

                    b.HasIndex("ImageId");

                    b.ToTable("Quizzes");
                });

            modelBuilder.Entity("QuizAI.Domain.Entities.TrueFalseAnswer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsCorrect")
                        .HasColumnType("bit");

                    b.Property<int>("QuestionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId")
                        .IsUnique();

                    b.ToTable("TrueFalseAnswers");
                });

            modelBuilder.Entity("CategoryQuiz", b =>
                {
                    b.HasOne("QuizAI.Domain.Entities.Category", null)
                        .WithMany()
                        .HasForeignKey("CategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QuizAI.Domain.Entities.Quiz", null)
                        .WithMany()
                        .HasForeignKey("QuizzesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("QuizAI.Domain.Entities.MultipleChoiceAnswer", b =>
                {
                    b.HasOne("QuizAI.Domain.Entities.Question", "Question")
                        .WithMany("MultipleChoiceAnswers")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");
                });

            modelBuilder.Entity("QuizAI.Domain.Entities.OpenEndedAnswer", b =>
                {
                    b.HasOne("QuizAI.Domain.Entities.Question", "Question")
                        .WithOne("OpenEndedAnswer")
                        .HasForeignKey("QuizAI.Domain.Entities.OpenEndedAnswer", "QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");
                });

            modelBuilder.Entity("QuizAI.Domain.Entities.Question", b =>
                {
                    b.HasOne("QuizAI.Domain.Entities.Image", "Image")
                        .WithMany("Questions")
                        .HasForeignKey("ImageId");

                    b.HasOne("QuizAI.Domain.Entities.Quiz", "Quiz")
                        .WithMany("Questions")
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Image");

                    b.Navigation("Quiz");
                });

            modelBuilder.Entity("QuizAI.Domain.Entities.Quiz", b =>
                {
                    b.HasOne("QuizAI.Domain.Entities.Image", "Image")
                        .WithMany("Quizzes")
                        .HasForeignKey("ImageId");

                    b.Navigation("Image");
                });

            modelBuilder.Entity("QuizAI.Domain.Entities.TrueFalseAnswer", b =>
                {
                    b.HasOne("QuizAI.Domain.Entities.Question", "Question")
                        .WithOne("TrueFalseAnswer")
                        .HasForeignKey("QuizAI.Domain.Entities.TrueFalseAnswer", "QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");
                });

            modelBuilder.Entity("QuizAI.Domain.Entities.Image", b =>
                {
                    b.Navigation("Questions");

                    b.Navigation("Quizzes");
                });

            modelBuilder.Entity("QuizAI.Domain.Entities.Question", b =>
                {
                    b.Navigation("MultipleChoiceAnswers");

                    b.Navigation("OpenEndedAnswer")
                        .IsRequired();

                    b.Navigation("TrueFalseAnswer")
                        .IsRequired();
                });

            modelBuilder.Entity("QuizAI.Domain.Entities.Quiz", b =>
                {
                    b.Navigation("Questions");
                });
#pragma warning restore 612, 618
        }
    }
}
