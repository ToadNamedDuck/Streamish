using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Streamish.Models;
using Streamish.Utils;
using System;
using System.Collections.Generic;

namespace Streamish.Repositories
{
    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository(IConfiguration configuration) : base(configuration)
        {

        }
        public List<UserProfile> GetAll()
        {
            using (var connection = Connection)
            {
                List<UserProfile> users = new();
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select Id, [Name], Email, ImageUrl, DateCreated from UserProfile";

                    using(SqlDataReader reader =  cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(UserBuilder(reader));
                        }
                    }
                }
                return users;
            }
        }
        public UserProfile GetById(int id)
        {
            UserProfile user = null;
            using (var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select Id, [Name], Email, ImageUrl, DateCreated from UserProfile Where Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            user = UserBuilder(reader);
                        }
                    }
                }
            }
            return user;
        }
        public void Add(UserProfile profile)
        {
            using(var connection = Connection)
            {
                connection.Open();
                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Insert into UserProfile ([Name], Email, ImageUrl, DateCreated)
                                        OUTPUT Inserted.ID
                                        values (@name, @email, @imageUrl, @dateCreated)";

                    cmd.Parameters.AddWithValue("@name", profile.Name);
                    cmd.Parameters.AddWithValue("@email", profile.Email);
                    if(profile.ImageUrl != null)
                    {
                        cmd.Parameters.AddWithValue("@imageUrl", profile.ImageUrl);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("imageUrl", DBNull.Value);
                    }
                    cmd.Parameters.AddWithValue("@dateCreated", DateTime.Now);

                    profile.Id = (int)cmd.ExecuteScalar();
                }
            }
        }
        public void Update(UserProfile profile)
        {
            using(var connection = Connection)
            {
                connection.Open();
                using( var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Update UserProfile
                                        Set Name = @name,
                                        Email = @email,
                                        ImageUrl = @imageUrl,
                                        DateCreated = @dateCreated
                                        Where Id = @id";
                    DbUtils.AddParameter(cmd, "@name", profile.Name);
                    DbUtils.AddParameter(cmd, "@email", profile.Email);
                    DbUtils.AddParameter(cmd, "@imageUrl", profile.ImageUrl);
                    DbUtils.AddParameter(cmd, "@dateCreated", profile.DateCreated);
                    DbUtils.AddParameter(cmd, "@id", profile.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete(int id)
        {
            using (var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Delete from UserProfile where Id = @id";
                    DbUtils.AddParameter(cmd, "@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public UserProfile GetByIdWithVideos(int id)
        {
            UserProfile user = null;
            using (var connection = Connection)
            {
                connection.Open();
                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select u.Id, u.Name, u.Email, u.ImageUrl, u.DateCreated, 

                                        v.Id as VideoId, v.Title, v.Description, v.Url, v.DateCreated as VideoDate,
                                        v.UserProfileId
                                        
                                        from UserProfile u
                                        left join Video v

                                        on v.UserProfileId = u.Id
                                        where u.Id = @id";

                    DbUtils.AddParameter(cmd, "@id", id);

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            if(user == null)
                            {
                                user = UserBuilder(reader);
                            }

                            Video video = new()
                            {
                                Id = DbUtils.GetInt(reader, "VideoId"),
                                Title = DbUtils.GetString(reader, "Title"),
                                Description = DbUtils.GetString(reader, "Description"),
                                Url = DbUtils.GetString(reader, "Url"),
                                DateCreated = DbUtils.GetDateTime(reader, "VideoDate"),
                                UserProfileId = DbUtils.GetInt(reader, "UserProfileId")
                            };
                            user.Videos.Add(video);
                        }
                    }
                }
            }
            return user;
        }

        private UserProfile UserBuilder(SqlDataReader reader)
        {
            UserProfile profile = new()
            {
                Id = DbUtils.GetInt(reader, "Id"),
                Name = DbUtils.GetString(reader, "Name"),
                Email = DbUtils.GetString(reader, "Email"),
                ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                DateCreated = DbUtils.GetDateTime(reader, "DateCreated")
            };
            return profile;
        }
    }
}
