﻿namespace ClashRoyale.Server.Database.Models
{
    using System;
    using System.Threading.Tasks;

    using ClashRoyale.Server.Logic;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;

    internal class PlayerDb
    {
        [BsonId]                    internal BsonObjectId _id;

        [BsonElement("highId")]     internal int HighId;
        [BsonElement("lowId")]      internal int LowId;

        [BsonElement("profile")]    internal BsonDocument Profile;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerDb"/> class.
        /// </summary>
        internal PlayerDb()
        {
            // PlayerDb.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerDb"/> class.
        /// </summary>
        /// <param name="HighId">The high identifier.</param>
        /// <param name="LowId">The low identifier.</param>
        /// <param name="Profile">The profile.</param>
        internal PlayerDb(int HighId, int LowId, BsonDocument Profile = null)
        {
            this.HighId     = HighId;
            this.LowId      = LowId;
            this.Profile    = Profile;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerDb"/> class.
        /// </summary>
        /// <param name="Player">The player.</param>
        internal PlayerDb(Player Player)
        {
            this.HighId     = Player.HighId;
            this.LowId      = Player.LowId;
            this.Profile    = Player.ToBsonDocument();
        }

        /// <summary>
        /// Creates the specified player.
        /// </summary>
        /// <param name="Player">The player.</param>
        internal static async Task Create(Player Player)
        {
            await GameDb.Players.InsertOneAsync(new PlayerDb(Player));
        }

        /// <summary>
        /// Creates the player in the database.
        /// </summary>
        internal static async Task<PlayerDb> Save(Player Player)
        {
            var UpdatedEntity = await GameDb.Players.FindOneAndUpdateAsync(PlayerDb => PlayerDb.HighId == Player.HighId && PlayerDb.LowId == Player.LowId, Builders<PlayerDb>.Update.Set(PlayerDb => PlayerDb.Profile, Player.ToBsonDocument()));

            if (UpdatedEntity != null)
            {
                if (UpdatedEntity.HighId == Player.HighId && UpdatedEntity.LowId == Player.LowId)
                {
                    return UpdatedEntity;
                }
                else
                {
                    Logging.Error(typeof(PlayerDb), "UpdatedEntity.Ids != this.Ids at Save().");
                }
            }
            else
            {
                Logging.Error(typeof(PlayerDb), "UpdatedEntity == null at Save().");
            }

            return null;
        }

        /// <summary>
        /// Loads this instance from the database.
        /// </summary>
        internal static async Task<PlayerDb> Load(int HighId, int LowId)
        {
            if (LowId > 0)
            {
                var Entities = await GameDb.Players.FindAsync(Player => Player.HighId == HighId && Player.LowId == LowId);

                if (Entities != null)
                {
                    var Entity = Entities.FirstOrDefault();

                    if (Entity != null)
                    {
                        return Entity;
                    }
                    else
                    {
                        Logging.Error(typeof(PlayerDb), "Entity == null at Load().");
                    }
                }
                else
                {
                    Logging.Error(typeof(PlayerDb), "Entities == null at Load().");
                }
            }
            else
            {
                Logging.Error(typeof(PlayerDb), "this.LowId < 0 at Load().");
            }

            return null;
        }

        /// <summary>
        /// Deletes this instance from the database.
        /// </summary>
        internal static async Task<bool> Delete(int HighId, int LowId)
        {
            if (LowId > 0)
            {
                var Result = await GameDb.Players.DeleteOneAsync(PlayerDb => PlayerDb.HighId == HighId && PlayerDb.LowId == LowId);

                if (Result.IsAcknowledged)
                {
                    if (Result.DeletedCount > 0)
                    {
                        if (Result.DeletedCount == 1)
                        {
                            return true;
                        }
                        else
                        {
                            Logging.Error(typeof(PlayerDb), "Result.DeletedCount > 1 at Delete().");
                        }
                    }
                    else
                    {
                        Logging.Warning(typeof(PlayerDb), "Result.DeletedCount == 0 at Delete().");
                    }
                }
                else
                {
                    Logging.Error(typeof(PlayerDb), "Result.IsAcknowledged != true at Delete().");
                }
            }
            else
            {
                Logging.Error(typeof(PlayerDb), "LowId <= 0 at Delete(HighId, LowId).");
            }

            return false;
        }
    }
}