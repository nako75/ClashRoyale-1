﻿namespace ClashRoyale.Server.Logic.Spells
{
    using ClashRoyale.Server.Extensions;
    using ClashRoyale.Server.Extensions.Helper;
    using ClashRoyale.Server.Files.Csv.Logic;

    using Newtonsoft.Json.Linq;

    internal class Spell
    {
        internal SpellData Data;

        internal int Count;
        internal int Level;
        internal int CreateTime;
        internal int RecentUseCount;

        internal int NewCount;

        internal bool NewFlag;
        internal bool NewUpgrade;

        /// <summary>
        /// Gets the level index if all material used.
        /// </summary>
        internal int LevelIndexIfAllMaterialUsed
        {
            get
            {
                int Count = this.Count;
                int Level = this.Level;

                for (int I = this.Level; I < this.Data.MaxLevelIndex; I++)
                {
                    int MaterialCountForNextLevel = this.MaterialCountForNextLevel;

                    if (Count >= MaterialCountForNextLevel)
                    {
                        Count -= MaterialCountForNextLevel;
                        ++Level;
                    }
                    else
                    {
                        break;
                    }
                }

                return Level;
            }
        }

        /// <summary>
        /// Gets if this spell can be upgraded.
        /// </summary>
        internal bool CanUpgrade
        {
            get
            {
                return this.Count >= this.MaterialCountForNextLevel;
            }
        }

        /// <summary>
        /// Gets the max material count.
        /// </summary>
        internal int MaxMaterialCount
        {
            get
            {
                int Count = 0;

                for (int I = this.Level; I < this.Data.RarityData.UpgradeMaterialCount.Length; I++)
                {
                    Count += this.Data.RarityData.UpgradeMaterialCount[I];
                }

                return Count;
            }
        }

        /// <summary>
        /// Gets the necessary material cout for next level.
        /// </summary>
        internal int MaterialCountForNextLevel
        {
            get
            {
                if (this.Level >= this.Data.MaxLevelIndex)
                {
                    return this.Data.MaxLevelIndex;
                }

                return this.Data.RarityData.UpgradeMaterialCount[this.Level];
            }
        }

        /// <summary>
        /// Gets the number of xp gain for upgrade.
        /// </summary>
        internal int UpgradeExp
        {
            get
            {
                return this.Data.RarityData.UpgradeExp[this.Level];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Spell"/> class.
        /// </summary>
        internal Spell()
        {
            // Spell.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Spell"/> class.
        /// </summary>
        /// <param name="SpellData">The spell data.</param>
        internal Spell(SpellData SpellData)
        {
            this.Data = SpellData;
        }

        /// <summary>
        /// Adds the specified material count.
        /// </summary>
        internal int AddMaterial(int Count)
        {
            int MaxAdd  = Math.Max(this.MaxMaterialCount - this.Count, 0);
            int Add     = Math.Min(MaxAdd, Count);

            this.Count += Add;

            if (Add > 0)
            {
                if (this.Level < this.Data.MaxLevelIndex)
                {
                    this.NewUpgrade = this.Count >= this.MaterialCountForNextLevel;
                }
            }

            return Math.Max(Count - Add, 0);
        }

        /// <summary>
        /// Clears the new update upgrade available.
        /// </summary>
        internal void ClearNewUpgradeAvailable()
        {
            this.NewUpgrade = false;
        }

        /// <summary>
        /// Upgrades this spell to next level.
        /// </summary>
        internal void UpgradeToNextLevel()
        {
            if (this.CanUpgrade)
            {
                this.Count -= this.MaterialCountForNextLevel;
                ++this.Level;
                this.NewUpgrade = false;
            }
        }

        /// <summary>
        /// Adds the specified material count.
        /// </summary>
        internal void SetCreateTime(int CreateTime)
        {
            this.CreateTime = CreateTime;
        }

        /// <summary>
        /// Adds the specified material count.
        /// </summary>
        internal void SetMaterialCount(int Count)
        {
            this.Count = Count;
        }

        /// <summary>
        /// Sets if must be show the new count.
        /// </summary>
        internal void SetShownNewCount(int Count)
        {
            this.NewCount = Count;
        }

        /// <summary>
        /// Sets if must be show the new icon.
        /// </summary>
        internal void SetShowNewIcon(bool Show)
        {
            this.NewFlag = Show;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        internal Spell Clone()
        {
            return new Spell(this.Data)
            {
                Count           = this.Count,
                CreateTime      = this.CreateTime,
                Level           = this.Level,
                NewCount        = this.NewCount,
                NewFlag         = this.NewFlag,
                NewUpgrade      = this.NewUpgrade,
                RecentUseCount  = this.RecentUseCount
            };
        }

        /// <summary>
        /// Decodes this instance.
        /// </summary>
        internal void Decode(ByteStream Reader)
        {
            this.Data           = Reader.DecodeLogicData<SpellData>(26);
            this.Level          = Reader.ReadVInt();
            this.CreateTime     = Reader.ReadVInt();
            this.Count          = Reader.ReadVInt();
            this.NewCount       = Reader.ReadVInt();
            this.RecentUseCount = Reader.ReadVInt();
            this.NewUpgrade     = Reader.ReadBoolean();
            this.NewFlag        = Reader.ReadBoolean();
        }

        /// <summary>
        /// Decodes this instance.
        /// </summary>
        internal void DecodeAttack(ByteStream Reader)
        {
            this.Data  = Reader.DecodeLogicData<SpellData>(26);
            this.Level = Reader.ReadVInt();
        }

        /// <summary>
        /// Encodes this instance.
        /// </summary>
        internal void Encode(ChecksumEncoder Stream)
        {
            Stream.EncodeLogicData(this.Data, 26);
            Stream.WriteVInt(this.Level);
            Stream.WriteVInt(this.CreateTime);
            Stream.WriteVInt(this.Count);
            Stream.WriteVInt(this.NewCount);
            Stream.WriteVInt(this.RecentUseCount);
            Stream.WriteBoolean(this.NewUpgrade);
            Stream.WriteBoolean(this.NewFlag);
        }

        /// <summary>
        /// Encodes this instance.
        /// </summary>
        internal void EncodeAttack(ChecksumEncoder Stream)
        {
            Stream.EncodeLogicData(this.Data, 26);
            Stream.WriteVInt(this.Level);
        }

        /// <summary>
        /// Loads this instance from json.
        /// </summary>
        internal void Load(JToken Json)
        {
            JsonHelper.GetJsonData(Json, "d", out this.Data);
            JsonHelper.GetJsonNumber(Json, "t", out this.CreateTime);
            JsonHelper.GetJsonNumber(Json, "c", out this.Count);
            JsonHelper.GetJsonNumber(Json, "l", out this.Level);
            JsonHelper.GetJsonNumber(Json, "newc", out this.NewCount);
            JsonHelper.GetJsonNumber(Json, "rcnt", out this.RecentUseCount);
            JsonHelper.GetJsonBoolean(Json, "newf", out this.NewFlag);
            JsonHelper.GetJsonBoolean(Json, "newu", out this.NewUpgrade);
        }

        /// <summary>
        /// Saves this instance to json.
        /// </summary>
        internal JObject Save()
        {
            JObject Json = new JObject();

            if (this.Data != null)
            {
                Json.Add("d", this.Data.GlobalId);
            }

            Json.Add("t", this.CreateTime);
            Json.Add("c", this.Count);
            Json.Add("l", this.Level);
            Json.Add("newc", this.NewCount);
            Json.Add("rnct", this.RecentUseCount);

            Json.Add("newf", this.NewFlag);
            Json.Add("newu", this.NewUpgrade);

            return Json;
        }

        internal bool Equals(Spell Spell)
        {
            return Spell.Data == this.Data;
        }

        public override bool Equals(object Obj)
        {
            if (Obj is Spell Spell)
            {
                return Spell.Data == this.Data;
            }

            return false;
        }
    }
}