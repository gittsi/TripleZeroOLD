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
            RuleFor(player => player.Characters).NotEmpty().WithMessage("Characters are empty!!!");
            RuleFor(player => player.Ships).NotEmpty().WithMessage("Ships are empty!!!");            
        }        
    }
}
