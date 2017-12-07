using FluentValidation;
using SWGoH.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Bot.Validators
{
    public class CharacterValidator : AbstractValidator<Character>
    {
        public CharacterValidator()
        {
            RuleFor(character => character.Abilities).NotEmpty().When(p=>p.Level>0).WithMessage(x=> string.Format("Abilities in character {0} are empty!!!",x.Name));
            RuleFor(character => character.Gear).NotEmpty().When(p => p.Level > 0).WithMessage(x => string.Format("Gear in character {0} is empty!!!", x.Name));
            RuleFor(character => character.GeneralStats).NotEmpty().WithMessage(x => string.Format("General stats in character {0} are empty!!!", x.Name));
            //RuleFor(character => character.Level).NotEmpty().WithMessage(x => string.Format("Level in character {0} is empty!!!", x.Name));
            RuleFor(character => character.Name).NotEmpty().When(p => p.Level > 0).WithMessage(x=> string.Format("Name in character is empty!!!", x.Name));
            RuleFor(character => character.OffenseStats).NotEmpty().When(p => p.Level > 0).WithMessage(x => string.Format("OffenseStats in character {0} are empty!!!", x.Name));
            RuleFor(character => character.Survivability).NotEmpty().When(p => p.Level > 0).WithMessage(x => string.Format("Survivability stats in character {0} are empty!!!", x.Name));
            RuleFor(character => character.Stars).NotEmpty().When(p => p.Level > 0).WithMessage(x => string.Format("Stars in character {0} is empty!!!", x.Name));
            //RuleFor(x => x.).SetCollectionValidator(new CharacterValidator());
        }        
    }
}
