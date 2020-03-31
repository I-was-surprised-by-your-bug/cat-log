using CatLog.Api.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CatLog.Api.Data.Contexts
{
    public partial class CatLogContext : DbContext
    {
        public CatLogContext()
        {
        }

        public CatLogContext(DbContextOptions<CatLogContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Article> TArticles { get; set; }
        public virtual DbSet<Column> TColumns { get; set; }
        public virtual DbSet<Section> TSections { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.ToTable("t_articles");

                entity.HasIndex(e => e.ColumnId)
                    .HasName("column_id");

                entity.Property(e => e.Id)
                    .HasColumnName("article_id")
                    .HasColumnType("bigint(255)")
                    .HasComment("文章ID");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("article_content")
                    .HasColumnType("longtext")
                    .HasComment("内容")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Introduction)
                    .IsRequired()
                    .HasColumnName("article_introduction")
                    .HasColumnType("text")
                    .HasComment("引言")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LikeCount)
                    .HasColumnName("article_like_count")
                    .HasColumnType("bigint(20)")
                    .HasComment("点赞数");

                entity.Property(e => e.Time)
                    .HasColumnName("article_time")
                    .HasColumnType("datetime")
                    .HasComment("发表时间");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("article_title")
                    .HasColumnType("text")
                    .HasComment("标题")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ViewsCount)
                    .HasColumnName("article_views_count")
                    .HasColumnType("bigint(20)")
                    .HasComment("浏览量");

                entity.Property(e => e.ColumnId)
                    .HasColumnName("column_id")
                    .HasColumnType("bigint(20)")
                    .HasComment("栏目ID");

                entity.HasOne(d => d.Column)
                    .WithMany(p => p.TArticles)
                    .HasForeignKey(d => d.ColumnId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("t_articles_ibfk_1");
            });

            modelBuilder.Entity<Column>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.ToTable("t_columns");

                entity.HasIndex(e => e.Name)
                    .HasName("column_name");

                entity.HasIndex(e => e.SectionId)
                    .HasName("section_id");

                entity.Property(e => e.Id)
                    .HasColumnName("column_id")
                    .HasColumnType("bigint(20)")
                    .HasComment("栏目ID");

                entity.Property(e => e.Description)
                    .HasColumnName("column_description")
                    .HasColumnType("text")
                    .HasComment("描述")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("column_name")
                    .HasColumnType("varchar(20)")
                    .HasComment("名称")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.SectionId)
                    .HasColumnName("section_id")
                    .HasColumnType("bigint(20)")
                    .HasComment("板块ID");

                entity.HasOne(d => d.Section)
                    .WithMany(p => p.TColumns)
                    .HasForeignKey(d => d.SectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("t_columns_ibfk_1");
            });

            modelBuilder.Entity<Section>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.ToTable("t_sections");

                entity.HasIndex(e => e.Name)
                    .HasName("section_name");

                entity.Property(e => e.Id)
                    .HasColumnName("section_id")
                    .HasColumnType("bigint(20)")
                    .HasComment("板块ID");

                entity.Property(e => e.Description)
                    .HasColumnName("section_description")
                    .HasColumnType("text")
                    .HasComment("描述")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("section_name")
                    .HasColumnType("varchar(20)")
                    .HasComment("名称")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
