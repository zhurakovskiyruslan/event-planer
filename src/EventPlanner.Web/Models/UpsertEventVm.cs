 using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Web.Models;

 public record UpsertEventVm(
       string Title,
       string Description,
       DateTime StartAtUtc,
       int Capacity,
       int LocationId
       );
     