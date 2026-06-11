using ArdCRM.Core.Entities;
using FluentValidation;

namespace ArdCRM.Business.Validators;

public class TeklifValidator : AbstractValidator<Teklif>
{
    public TeklifValidator()
    {
        RuleFor(x => x.Baslik)
            .NotEmpty().WithMessage("Teklif başlığı zorunludur.")
            .MaximumLength(300).WithMessage("Başlık en fazla 300 karakter olabilir.");

        RuleFor(x => x.Tutar)
            .GreaterThan(0).WithMessage("Teklif tutarı sıfırdan büyük olmalıdır.");

        RuleFor(x => x.MusteriId)
            .GreaterThan(0).WithMessage("Geçerli bir müşteri seçiniz.");

        RuleFor(x => x.Para)
            .NotEmpty().WithMessage("Para birimi zorunludur.")
            .MaximumLength(3).WithMessage("Para birimi en fazla 3 karakter olabilir.");

        RuleFor(x => x.GecerlilikTarihi)
            .GreaterThanOrEqualTo(x => x.TeklifTarihi)
            .WithMessage("Geçerlilik tarihi, teklif tarihinden önce olamaz.")
            .When(x => x.GecerlilikTarihi.HasValue);
    }
}
