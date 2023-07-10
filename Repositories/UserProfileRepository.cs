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

                    cmd.ExecuteNonQuery();
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
