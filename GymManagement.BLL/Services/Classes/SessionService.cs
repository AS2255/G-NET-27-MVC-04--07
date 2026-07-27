using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Data.Models.Enums;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISessionRepository _sessionRepo;

        public SessionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sessionRepo = _unitOfWork.SessionRepository;
        }

        public async Task<Result<IEnumerable<SessionViewModel>>> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessions = await _sessionRepo.GetSessionsWithIncludeAsync(ct:ct);
            if (sessions == null || !sessions.Any()) return Result<IEnumerable<SessionViewModel>>.Ok([]);

            var mappedSessions = new List<SessionViewModel>();

            foreach (var session in sessions)
            {
                var mappedSession = _mapper.Map<Session, SessionViewModel>(session);

                mappedSession.AvailableSlots = session.Capacity - await _sessionRepo.GetBookedSessionsCountAsync(session.Id, ct);

                mappedSessions.Add(mappedSession);
            }

            return Result<IEnumerable<SessionViewModel>>.Ok(mappedSessions);

        }

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (model.StartDate >= model.EndDate) return Result.Validation("Start Date Cannot Be After End Date");
            if (model.StartDate <= DateTime.Now) return Result.Validation("Start Date Cannot Be In The Past");
            if (model.Capacity < 1 || model.Capacity > 25) return Result.Validation("Capacity Must Be Between 1 And 25");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId, ct);
            if (trainer is null) return Result.Validation("Trainer Does Not Exist");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId, ct);
            if (category is null) return Result.Validation("Category Does Not Exist");

            if (!Enum.TryParse<Speciality>(category.CategoryName, true, out var categorySpeciality))
                return Result.Validation($"{category.CategoryName} Is Not A Speciality");

            if (trainer.Speciality != categorySpeciality) return Result.Validation("Trainer Speciality And Chosen Category Do Not Match");

            //Ensure trainer doesn't have two sessions on the same date (double booking)
            //if new session ends AFTER existing session startDate   AND   new session begins BEFORE existing session endDate
            var doubleBooking = await _unitOfWork.GetRepository<Session>().AnyAsync(x => x.TrainerId == model.TrainerId && (model.EndDate >= x.StartDate && model.StartDate <= x.EndDate), ct);

            if (doubleBooking) return Result.Validation("Trainer Is Already Booked For That Time Slot");

            _unitOfWork.GetRepository<Session>().AddAsync(_mapper.Map<CreateSessionViewModel, Session>(model));

            return await _unitOfWork.SaveChangesAsync(ct) > 0 ? Result.Ok() : Result.Fail("Failed To Create Session");
        }

        public async Task<Result<SessionViewModel>> GetSessionByIdAsync(int id, CancellationToken ct = default)
        {
            var session = await _sessionRepo.GetSessionByIdWithIncludeAsync(id, ct);
            if (session is null) return Result<SessionViewModel>.NotFound($"Session With Id {id} Is Not Found!");

            var mappedSession = _mapper.Map<SessionViewModel>(session);

            var bookedSlots = await _unitOfWork.SessionRepository.GetBookedSessionsCountAsync(id, ct);
            mappedSession.AvailableSlots = mappedSession.Capacity - bookedSlots;
            return Result<SessionViewModel>.Ok(mappedSession);
        }

        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int id, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(id, ct);
            if (session is null)
                return Result<UpdateSessionViewModel>.NotFound($"Session With Id {id} Is Not Found!");

            if (DateTime.Now >= session.StartDate)
                return Result<UpdateSessionViewModel>.Fail($"Cannot Update Session With Id {id}, Session Is Completed Or Ongoing!");

            var isBooked = await _unitOfWork.SessionRepository.GetBookedSessionsCountAsync(id, ct) > 0 ? true : false;
            if (isBooked)
                return Result<UpdateSessionViewModel>.Fail($"Cannot Update Session With Id {id}, Session Is Booked By Members!");

            var mappedSession = _mapper.Map<UpdateSessionViewModel>(session);

            return Result<UpdateSessionViewModel>.Ok(mappedSession);
        }

        public async Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(id, ct);
            if (session is null)
                return Result.NotFound($"Session With Id {id} Is Not Found!");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail($"Cannot Edit Session With Id {id}, Session Already Started!");

            if (model.EndDate <= model.StartDate)
                return Result.Validation($"Cannot Edit Session With Id {id}, End Date Must Be After Start Date!");

            var isBooked = await _unitOfWork.SessionRepository.GetBookedSessionsCountAsync(id, ct) > 0 ? true : false;
            if (isBooked)
                return Result.Fail($"Cannot Update Session With Id {id}, Session Is Booked By Members!");

            if(model.StartDate <= DateTime.Now)
                return Result.Validation($"Cannot Edit Session With Id {id}, Start Date Must Be In The Future!");


            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId, ct);
            if (trainer is null) return Result.Validation("Trainer Does Not Exist");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId, ct);
           
            if (!Enum.TryParse<Speciality>(category!.CategoryName, true, out var categorySpeciality))
                return Result.Validation($"{category.CategoryName} Is Not A Speciality");

            if (trainer.Speciality != categorySpeciality) return Result.Validation("Trainer Speciality And Chosen Category Do Not Match");

            _mapper.Map(model, session);
            session.UpdatedAt = DateTime.Now;

            _unitOfWork.SessionRepository.UpdateAsync(session);
            var res = await _unitOfWork.SaveChangesAsync();

            return res > 0 ? Result.Ok() : Result.Fail($"Failed To Update Session With Id {id}");
        }

        public async Task<Result> DeleteSessionAsync(int id, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(id, ct);
            if (session is null)
                return Result.NotFound($"Session With Id {id} Is Not Found!");

            if (DateTime.Now <= session.EndDate)
                return Result.Fail($"Cannot Delete Session With Id {id}, Session Is Upcoming Or Ongoing!");

            var isBooked = await _unitOfWork.SessionRepository.GetBookedSessionsCountAsync(id, ct) > 0 ? true : false;
            if (isBooked)
                return Result.Fail($"Cannot Delete Session With Id {id}, Session Is Booked By Members!");

            _unitOfWork.SessionRepository.DeleteAsync(session);
            var res = await _unitOfWork.SaveChangesAsync(ct);

            return  res > 0 ? Result.Ok() : Result.Fail($"Failed To Delete Session With Id {id}");
        }

        public async Task<Result<IEnumerable<TrainerSelectViewModel>>> GetTrainerSelectListAsync(CancellationToken ct = default)
            => Result<IEnumerable<TrainerSelectViewModel>>.Ok(_mapper.Map<IEnumerable<Trainer>, IEnumerable<TrainerSelectViewModel>>(await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct)));


        public async Task<Result<IEnumerable<CategorySelectViewModel>>> GetCategorySelectListAsync(CancellationToken ct = default)
            => Result<IEnumerable<CategorySelectViewModel>>.Ok(_mapper.Map<IEnumerable<Category>, IEnumerable<CategorySelectViewModel>>(await _unitOfWork.GetRepository<Category>().GetAllAsync(ct: ct)));
    }
}
