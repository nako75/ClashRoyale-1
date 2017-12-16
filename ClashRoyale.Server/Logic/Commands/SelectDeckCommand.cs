﻿namespace ClashRoyale.Server.Logic.Commands
{
    using ClashRoyale.Server.Extensions;
    using ClashRoyale.Server.Extensions.Game;
    using ClashRoyale.Server.Logic.Mode;

    internal class SelectDeckCommand : Command
    {
        internal int DeckIdx;

        /// <summary>
        /// Gets the type of this command.
        /// </summary>
        internal override int Type
        {
            get
            {
                return 500;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectDeckCommand"/> class.
        /// </summary>
        public SelectDeckCommand()
        {
            // SelectDeckCommand.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectDeckCommand"/> class.
        /// </summary>
        public SelectDeckCommand(int DeckIdx)
        {
            this.DeckIdx = DeckIdx;
        }

        /// <summary>
        /// Decodes this instance.
        /// </summary>
        internal override void Decode(ByteStream Stream)
        {
            base.Decode(Stream);

            this.DeckIdx = Stream.ReadVInt();
        }

        /// <summary>
        /// Encodes this instance.
        /// </summary>
        internal override void Encode(ChecksumEncoder Stream)
        {
            base.Encode(Stream);
            
            Stream.WriteVInt(this.DeckIdx);
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        internal override byte Execute(GameMode GameMode)
        {
            if (Globals.MultipleDecks)
            {
                if (this.DeckIdx > -1 && this.DeckIdx < 5)
                {
                    GameMode.Home.SetSelectedDeck(this.DeckIdx);

                    return 0;
                }

                return 2;
            }

            return 1;
        }
    }
}