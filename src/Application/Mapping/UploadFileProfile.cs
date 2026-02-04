using Application.DTOs.TestDTO;
using AutoMapper;
using Domain;

namespace Application.Mapping;

public class UploadFileProfile:Profile
{
    public UploadFileProfile()
    {
        // 🔹 UPLOAD FILE: DTO -> Entity
        CreateMap<UploadFileDTO, UploadFile>()
            .ForMember(d => d.FileName, o => o.MapFrom(s => s.FileName))
            .ForMember(d => d.FileUrl, o => o.Ignore());

    }

}
