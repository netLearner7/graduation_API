using FluentValidation;
using FluentValidation.Results;
using Infrastructure.Dto;
using System;

namespace Infrastructure.Fluentvalidation
{
    public class MeetingDtoValidation : AbstractValidator<MeetingOutputDto>
    {
        public MeetingDtoValidation()
        {
            RuleFor(x => x.MeetingName).NotNull().WithMessage("会议名称不能为空");
            RuleFor(x => x.Address).NotNull().WithMessage("会议地址不能为空");
            RuleFor(x => x.MeetingState).NotNull().WithMessage("会议状态不能为空");
        }


    }
}
