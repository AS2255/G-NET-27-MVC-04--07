using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using static System.Net.WebRequestMethods;

namespace GymManagement.BLL.Services.Classes
{
    public class TrainersService : ITrainersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TrainersService(IUnitOfWork unitOfWork, IMapper mapper)
        {

            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<TrainerViewModel>>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync();
            if (trainers is null) return Result<IEnumerable<TrainerViewModel>>.Ok([]);

            var mappedTrainers = _mapper.Map<IEnumerable<Trainer>, IEnumerable<TrainerViewModel>>(trainers);

            return Result<IEnumerable<TrainerViewModel>>.Ok(mappedTrainers);

        }

        public async Task<Result<TrainerDetailsViewModel>> GetTrainerDetailsAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null) return Result<TrainerDetailsViewModel>.NotFound($"Trainer With Id {id} Not Found!");

            var mappedTrainer = _mapper.Map<Trainer, TrainerDetailsViewModel>(trainer);
            return Result<TrainerDetailsViewModel>.Ok(mappedTrainer);
        }

        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel trainer, CancellationToken ct = default)
        {
            var emailExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Email == trainer.Email);
            var phoneExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Phone == trainer.Phone);

            if (emailExists || phoneExists) return Result.Fail("Failed To Create Trainer, Email Or Phone Number Already Exists!");

            var createdTrainer = _mapper.Map<CreateTrainerViewModel, Trainer>(trainer);

            _unitOfWork.GetRepository<Trainer>().AddAsync(createdTrainer);
            var res = await _unitOfWork.SaveChangesAsync(ct);
            return res > 0 ? Result.Ok() : Result.Fail("Failed To Create Trainer!");
        }

        public async Task<Result<TrainerToUpdateViewModel>> GetTrainerToUpdateAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null) return Result<TrainerToUpdateViewModel>.NotFound($"Failed To Update Trainer With Id {id}, Not Found!");

            var mappedTrainer = _mapper.Map<Trainer, TrainerToUpdateViewModel>(trainer);

            return Result<TrainerToUpdateViewModel>.Ok(mappedTrainer);
        }

        public async Task<Result> UpdateTrainerAsync(int id, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null) return Result.NotFound($"Failed To Update Trainer With Id {id}, Not Found!");

            var emailExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Email == model.Email && x.Id != id);
            var phoneExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Phone == model.Phone && x.Id != id);

            if (emailExists || phoneExists) return Result.Fail("Failed To Update Trainer, Email Or Phone Number Already Exists!");

            _mapper.Map(model, trainer);

            _unitOfWork.GetRepository<Trainer>().UpdateAsync(trainer);
            var res = await _unitOfWork.SaveChangesAsync(ct);

            return res > 0 ? Result.Ok() : Result.Fail($"Failed To Update Trainer With Id {id}!");
        }

        public async Task<Result> RemoveTrainerAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            var assigned = await _unitOfWork.GetRepository<Session>().AnyAsync(x => x.TrainerId == id);
            if (trainer is null || assigned) return Result.Fail($"Failed To Remove Trainer With Id {id}, Not Found Or Assigned To Session!");

            _unitOfWork.GetRepository<Trainer>().DeleteAsync(trainer);
            var res = await _unitOfWork.SaveChangesAsync(ct);
            return res > 0 ? Result.Ok() : Result.Fail($"Failed To Remove Trainer With Id {id}!");
        }
    }
}
