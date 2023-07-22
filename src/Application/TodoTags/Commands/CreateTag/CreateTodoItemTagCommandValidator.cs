﻿using FluentValidation;
using Todo_App.Application.Common.Interfaces;

namespace Todo_App.Application.TodoTags.Commands.CreateTag;
public class CreateTodoItemTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTodoItemTagCommandValidator( )
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage("Please provide a name for your tag");
    }
}