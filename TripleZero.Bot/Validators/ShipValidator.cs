using FluentValidation;
using SWGoH.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Bot.Validators
{
    public class ShipValidator : AbstractValidator<Ship>
    {
        public ShipValidator()
        {
            //RuleFor(character => character.Abilities).NotEmpty().WithMessage("Abilities in character are empty!!!");            
            //RuleFor(character => character.GeneralStats).NotEmpty().WithMessage("General stats in character are empty!!!");
            //RuleFor(ship => ship.Level).NotEmpty().WithMessage("Level in ship is empty!!!");
            RuleFor(ship => ship.Name).NotEmpty().When(p => p.Level > 0).WithMessage("Name in ship is empty!!!");
            RuleFor(ship => ship.Stars).NotEmpty().When(p => p.Level > 0).WithMessage("Stars in ship is empty!!!");            
            //RuleFor(character => character.OffenseStats).NotEmpty().WithMessage("OffenseStats in character are empty!!!");
            //RuleFor(character => character.Survivability).NotEmpty().WithMessage("Survivability stats in character are empty!!!");
            //RuleFor(x => x.).SetCollectionValidator(new CharacterValidator());
        }        
    }
}
