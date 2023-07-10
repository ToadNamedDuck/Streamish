using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Streamish.Models;
using Streamish.Utils;

namespace Streamish.Repositories
{

    public class VideoRepository : BaseRepository, IVideoRepository
    {
        public VideoRepository(IConfiguration configuration) : base(configuration) { }

        public List<Video> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
               SELECT v.Id, v.Title, v.Description, v.Url, v.DateCreated, v.UserProfileId,

                      up.Name, up.Email, up.DateCreated AS UserProfileDateCreated,
                      up.ImageUrl AS UserProfileImageUrl
                        
                 FROM Video v 
                      JOIN UserProfile up ON v.UserProfileId = up.Id
             ORDER BY DateCreated
            ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        var videos = new List<Video>();
                        while (reader.Read())
                        {
                            videos.Add(new Video()
                            {
                                Id = DbUtils.GetInt(reader, "Id"),
                                Title = DbUtils.GetString(reader, "Title"),
                                Description = DbUtils.GetString(reader, "Description"),
                                Url = DbUtils.GetString(reader, "Url"),
                                DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                                UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
                                UserProfile = new UserProfile()
                                {
                                    Id = DbUtils.GetInt(reader, "UserProfileId"),
                                    Name = DbUtils.GetString(reader, "Name"),
                                    Email = DbUtils.GetString(reader, "Email"),
                                    DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                    ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                                },
                            });
                        }

                        return videos;
                    }
                }
            }
        }

        public Video GetById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                          SELECT v.Title, v.Description, v.Url, v.DateCreated, v.UserProfileId,
                          
                          up.Name, up.Email, up.DateCreated AS UserProfileDateCreated, up.ImageUrl AS UserProfileImageUrl

                            FROM Video v
                            JOIN UserProfile up ON v.UserProfileId = up.Id
                           WHERE v.Id = @Id";

                    DbUtils.AddParameter(cmd, "@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        Video video = null;
                        if (reader.Read())
                        {
                            video = new Video()
                            {
                                Id = id,
                                Title = DbUtils.GetString(reader, "Title"),
                                Description = DbUtils.GetString(reader, "Description"),
                                DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                                Url = DbUtils.GetString(reader, "Url"),
                                UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
                                UserProfile = new UserProfile()//attaching user profile object to the video object
                                {
                                    Id = DbUtils.GetInt(reader, "UserProfileId"),
                                    Name = DbUtils.GetString(reader, "Name"),
                                    Email = DbUtils.GetString(reader, "Email"),
                                    DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                    ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                                }
                            };
                        }

                        return video;
                    }
                }
            }
        }

        public void Add(Video video)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Video (Title, Description, DateCreated, Url, UserProfileId)
                        OUTPUT INSERTED.ID
                        VALUES (@Title, @Description, @DateCreated, @Url, @UserProfileId)";

                    DbUtils.AddParameter(cmd, "@Title", video.Title);
                    DbUtils.AddParameter(cmd, "@Description", video.Description);
                    DbUtils.AddParameter(cmd, "@DateCreated", video.DateCreated);
                    DbUtils.AddParameter(cmd, "@Url", video.Url);
                    DbUtils.AddParameter(cmd, "@UserProfileId", video.UserProfileId);

                    video.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(Video video)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE Video
                           SET Title = @Title,
                               Description = @Description,
                               DateCreated = @DateCreated,
                               Url = @Url,
                               UserProfileId = @UserProfileId
                         WHERE Id = @Id";

                    DbUtils.AddParameter(cmd, "@Title", video.Title);
                    DbUtils.AddParameter(cmd, "@Description", video.Description);
                    DbUtils.AddParameter(cmd, "@DateCreated", video.DateCreated);
                    DbUtils.AddParameter(cmd, "@Url", video.Url);
                    DbUtils.AddParameter(cmd, "@UserProfileId", video.UserProfileId);
                    DbUtils.AddParameter(cmd, "@Id", video.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Video WHERE Id = @Id";
                    DbUtils.AddParameter(cmd, "@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Video> GetAllWithComments()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT v.Id AS VideoId, v.Title, v.Description, v.Url, 
                       v.DateCreated AS VideoDateCreated, v.UserProfileId As VideoUserProfileId,

                       up.Name, up.Email, up.DateCreated AS UserProfileDateCreated,
                       up.ImageUrl AS UserProfileImageUrl,
                        
                       c.Id AS CommentId, c.Message, c.UserProfileId AS CommentUserProfileId
                  FROM Video v 
                       JOIN UserProfile up ON v.UserProfileId = up.Id
                       LEFT JOIN Comment c on c.VideoId = v.id
             ORDER BY  v.DateCreated
            ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        var videos = new List<Video>();//list of videos
                        while (reader.Read())
                        {
                            var videoId = DbUtils.GetInt(reader, "VideoId");//grab the current row's video id

                            var existingVideo = videos.FirstOrDefault(p => p.Id == videoId);//check the list of videos to see if we have a video in the list there already
                            if (existingVideo == null)//if not, make a new video object to add to the list.
                            {
                                existingVideo = new Video()
                                {
                                    Id = videoId,
                                    Title = DbUtils.GetString(reader, "Title"),
                                    Description = DbUtils.GetString(reader, "Description"),
                                    DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
                                    Url = DbUtils.GetString(reader, "Url"),
                                    UserProfileId = DbUtils.GetInt(reader, "VideoUserProfileId"),
                                    UserProfile = new UserProfile()//attaching user profile object to the video object
                                    {
                                        Id = DbUtils.GetInt(reader, "VideoUserProfileId"),
                                        Name = DbUtils.GetString(reader, "Name"),
                                        Email = DbUtils.GetString(reader, "Email"),
                                        DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                        ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                                    },
                                    Comments = new List<Comment>()//attaching the list of a videos comments. it is currently empty.
                                };

                                videos.Add(existingVideo);//add video to list
                            }//end of if statement that generates a new comment entry is one doesnt already exist.

                            if (DbUtils.IsNotDbNull(reader, "CommentId"))//after the if statement, always check the row for a comment id
                            {
                                existingVideo.Comments.Add(new Comment()//if we find one, we add a new comment object to the list inside of the video object we may have just made,
                                                                        //or already made previously
                                {
                                    Id = DbUtils.GetInt(reader, "CommentId"),
                                    Message = DbUtils.GetString(reader, "Message"),
                                    VideoId = videoId,
                                    UserProfileId = DbUtils.GetInt(reader, "CommentUserProfileId")
                                });
                            }//end of current loop cycle, continue if more rows, exit out and return if not
                        }

                        return videos;
                    }
                }
            }
        }

        public Video GetVideoByIdWithComments(int id)
        {
            Video video = null;
            using(var connection = Connection)
            {
                connection.Open();
                using(var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                                            Select v.Title, v.Description, v.Url, v.DateCreated as VideoCreationDate, v.UserProfileId as VideoPosterId,
                                            
                                            c.Id as CommentId, c.Message, c.UserProfileId as CommentPosterId

                                            from Video v
                                            LEFT JOIN Comment c
                                            on c.VideoId = v.Id
                                            where v.Id = @id";

                    command.Parameters.AddWithValue("@id", id);

                    using(var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            if(video == null)
                            {
                                video = new()
                                {
                                    Id = id,
                                    Title = DbUtils.GetString(reader, "Title"),
                                    Description = DbUtils.GetString(reader, "Description"),
                                    Url = DbUtils.GetString(reader, "Url"),
                                    DateCreated = DbUtils.GetDateTime(reader, "VideoCreationDate"),
                                    UserProfileId = DbUtils.GetInt(reader, "VideoPosterId"),
                                    Comments = new()
                                };
                            }
                            if(video != null && DbUtils.IsNotDbNull(reader, "CommentId"))
                            {
                                Comment comment = new()
                                {
                                    Id = DbUtils.GetInt(reader, "CommentId"),
                                    Message = DbUtils.GetString(reader, "Message"),
                                    UserProfileId = DbUtils.GetInt(reader, "CommentPosterId")
                                };

                                video.Comments.Add(comment);
                            }
                        }
                    }
                }
            }
            return video;
        }

        public List<Video> Search(string criterion, bool sortDescending)//enables search using the LIKE keyword
            //criterion is the string which the user searches, and sort descending is simply a booleon like "/?sortDescending=true"
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    var sql = @"
              SELECT v.Id, v.Title, v.Description, v.Url, v.DateCreated AS VideoDateCreated, v.UserProfileId,

                     up.Name, up.Email, up.DateCreated AS UserProfileDateCreated,
                     up.ImageUrl AS UserProfileImageUrl
                        
                FROM Video v 
                     JOIN UserProfile up ON v.UserProfileId = up.Id
               WHERE v.Title LIKE @Criterion OR v.Description LIKE @Criterion";

                    if (sortDescending)//latest videos first
                    {
                        sql += " ORDER BY v.DateCreated DESC";
                    }
                    else//older
                    {
                        sql += " ORDER BY v.DateCreated";
                    }

                    cmd.CommandText = sql;//set the command text after modification via boolean
                    DbUtils.AddParameter(cmd, "@Criterion", $"%{criterion}%");//set the @Criterion param = to the search but with wildcards before and after.
                    //if you search "B", it should return anything where "B" is anywhere in any condition (by itself, in the front, middle, or end of a word)
                    //such as "Bee Movie" "WeB Du Bois", etc
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        var videos = new List<Video>();
                        while (reader.Read())
                        {
                            videos.Add(new Video()
                            {
                                Id = DbUtils.GetInt(reader, "Id"),
                                Title = DbUtils.GetString(reader, "Title"),
                                Description = DbUtils.GetString(reader, "Description"),
                                DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
                                Url = DbUtils.GetString(reader, "Url"),
                                UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
                                UserProfile = new UserProfile()
                                {
                                    Id = DbUtils.GetInt(reader, "UserProfileId"),
                                    Name = DbUtils.GetString(reader, "Name"),
                                    Email = DbUtils.GetString(reader, "Email"),
                                    DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                    ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                                },
                            });
                        }

                        return videos;
                    }
                }
            }
        }

        public List<Video> Hottest(DateTime time)
        {
            var videos = new List<Video>();
            using (var connection = Connection)
            {
                connection.Open();
                using(var cmd =  connection.CreateCommand())
                {
                    cmd.CommandText =
                @"SELECT count(c.VideoId), v.Id as VideoId, v.Title, v.Description, v.Url, v.DateCreated AS VideoDateCreated, v.UserProfileId,

                     up.Name, up.Email, up.DateCreated AS UserProfileDateCreated,
                     up.ImageUrl AS UserProfileImageUrl
                        
                FROM Video v 
                     JOIN UserProfile up ON v.UserProfileId = up.Id
                        Left Join Comment c ON c.VideoId = v.Id
                where v.DateCreated >= @postedOnOrAfter
                group by v.Id, v.Title, v.Description, v.Url, v.DateCreated, v.UserProfileId, up.Name,
                up.Email, up.DateCreated, up.ImageUrl, c.VideoId
                order by count(c.VideoId) desc";

                DbUtils.AddParameter(cmd, "@postedOnOrAfter", time);

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Video video = new()
                            {
                                Id = DbUtils.GetInt(reader, "VideoId"),
                                Title = DbUtils.GetString(reader, "Title"),
                                DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
                                Description = DbUtils.GetString(reader, "Description"),
                                Url = DbUtils.GetString(reader, "Url"),
                                UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
                                UserProfile = new()
                                {
                                    Id = DbUtils.GetInt(reader, "UserProfileId"),
                                    Email = DbUtils.GetString(reader, "Email"),
                                    DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                    ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl")
                                }
                            };

                            videos.Add(video);
                        }
                    }
                }

            }
            return videos;
        }
    }
}