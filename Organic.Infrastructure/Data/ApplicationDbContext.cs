using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Organic.Core.Entities
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		// DbSet properties represent tables in the database
		public DbSet<ScheduledPost> ScheduledPosts { get; set; }
		public DbSet<MediaFile> MediaFiles { get; set; }
		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<SocialMediaAccount> SocialMediaAccounts { get; set; }
		public DbSet<Campaign> Campaigns { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Campaign>(campaign =>
			{
				//campaign.HasOne(c => c.CreatedBy)
				//	    .WithMany(m => m.CreatedCampaigns)
				//	    .HasForeignKey(c => c.CreatedById)
				//		.OnDelete(DeleteBehavior.Restrict);

				campaign.HasKey(c => c.Id);

				campaign.HasMany(c => c.ApplicationUsers)
						.WithMany(v => v.Campaigns)
						.UsingEntity(j => j.ToTable("UserCampaigns"));

				campaign.HasOne(c => c.CreatedBy)
						.WithMany(au => au.CreatedCampaigns)
						.HasForeignKey(c => c.CreatedById)
						.OnDelete(DeleteBehavior.Restrict);

				//campaign.HasMany(c => c.SocialMediaAccounts)
				//	    .WithOne()
				//	    .HasForeignKey(sma => sma.CampaignId)
				//	    .OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<ScheduledPost>(scheduledPost =>
			{
				//scheduledPost.HasOne(sp => sp.SocialMediaAccount)
				//	         .WithMany()
				//			 .HasForeignKey(sp => sp.SocialMediaAccountId)
				//			 .OnDelete(DeleteBehavior.Cascade);

				//scheduledPost.HasMany(sp => sp.MediaFiles)
				//			 .WithOne()
				//			 .HasForeignKey(mf => mf.ScheduledPostId)
				//			 .OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<MediaFile>(mediaFile =>
			{
				//mediaFile.HasOne(mf => mf.ScheduledPost)
				//		 .WithMany(sp => sp.MediaFiles)
				//		 .HasForeignKey(mf => mf.ScheduledPostId)
				//		 .OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<SocialMediaAccount>(mediaAccount =>
			{
				mediaAccount.Property(sma => sma.AccessToken)
							.IsRequired();

				mediaAccount.Property(sma => sma.RefreshToken)
							.IsRequired();

				//mediaAccount.HasOne(ma => ma.Campaign)
				//			.WithMany()
				//			.HasForeignKey(ma => ma.CampaignId)
				//			.OnDelete(DeleteBehavior.Cascade);
			});
		}
	}
}
