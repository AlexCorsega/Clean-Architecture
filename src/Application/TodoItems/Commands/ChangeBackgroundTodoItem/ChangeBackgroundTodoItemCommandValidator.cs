using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Todo_App.Domain.ValueObjects;

namespace Todo_App.Application.TodoItems.Commands.ChangeBackgroundTodoItem;
public class ChangeBackgroundTodoItemCommandValidator : AbstractValidator<ChangeBackgroundTodoItemCommand>
{
    public ChangeBackgroundTodoItemCommandValidator()
    {
        RuleFor(s => s.BackgroundColour)
            .MaximumLength(7)
            .WithMessage("Please provide a valid color in hexadecimal.")
            .Must(ValidColor)
            .WithMessage("Invalid colour code.");
    }
    private bool ValidColor(string code)
    {
        try
        {
            return !string.IsNullOrEmpty(Colour.From(code).Code);
        }
        catch (Exception ex)
        {
            return false;
        }

    }
}
