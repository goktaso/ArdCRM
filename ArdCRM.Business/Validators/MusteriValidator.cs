using ArdCRM.Core.Entities;
using FluentValidation;

namespace ArdCRM.Business.Validators;

public class MusteriValidator : AbstractValidator<Musteri>
{
    public MusteriValidator()
    {
        RuleFor(x => x.Ad)
            .NotEmpty().WithMessage("Ad alanı zorunludur.")
            .MaximumLength(100).WithMessage("Ad en fazla 100 karakter olabilir.");

        RuleFor(x => x.FirmaAdi)
            .NotEmpty().WithMessage("Firma adı zorunludur.")
            .MaximumLength(200).WithMessage("Firma adı en fazla 200 karakter olabilir.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Telefon)
            .MaximumLength(20).WithMessage("Telefon en fazla 20 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.Telefon));

        RuleFor(x => x.VergiNo)
            .MaximumLength(20).WithMessage("Vergi no en fazla 20 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.VergiNo));
    }
}
