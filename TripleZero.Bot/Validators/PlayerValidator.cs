using FluentValidation;
using SWGoH.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Bot.Validators
{
    public class PlayerValidator : AbstractValidator<Player>
    {
        public PlayerValidator()
        {
            RuleSet("Basic", () =>
            {
                RuleFor(player => player.Characters).NotEmpty().WithMessage("Characters are empty!!!");
                RuleFor(player => player.Ships).NotEmpty().WithMessage("Ships are empty!!!");
            });

            RuleSet("WithCharacter", () =>
            {
                RuleFor(x => x.Characters).SetCollectionValidator(new CharacterValidator());
            });

            RuleSet("WithShip", () =>
            {
                RuleFor(x => x.Ships).SetCollectionValidator(new ShipValidator());
            });
        }        
    }
}
