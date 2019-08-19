using Deeproxio.Persistence.Configuration.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Deeproxio.Domain.Models
{
    public sealed class MediaSourceMap : BaseMap<MediaSource>
    {
        public MediaSourceMap()
        {
            BuilderFunc = entity =>
            {
                entity.ToTable("media_source");

                entity.HasIndex(e => e.Id)
                    .HasName("media_source_id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Name)
                    .HasName("media_source_name_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateTS).HasColumnName("createts");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.UpdateTS).HasColumnName("updatets");
            };
        }

        public override void Map(ModelBuilder builder)
        {
            base.Map(builder);

            builder.HasSequence("media_source_id_seq")
                .HasMin(1)
                .HasMax(2147483647);
        }
    }
}
