﻿using FluentValidation;
using Todo_App.Domain.ValueObjects;

namespace Todo_App.Application.TodoItems.Commands.UpdateBackgroundTodoItem;
public  class UpdateBackgroundTodoItemCommandValidator : AbstractValidator<UpdateBackgroundTodoItemCommand>
{
    public UpdateBackgroundTodoItemCommandValidator()
    {
        RuleFor(s => s.Colour)
        .Must(BeValidColour)
        .WithMessage("Background colour not found.")
        .MaximumLength(7)
        .WithMessage("Invalid colour code, please make sure it is in hexadecimal");
    }
    private bool BeValidColour(string color)
    {
        return Colour.IsValid(color);
    }
}
