using System;
using System.Collections.Generic;

namespace WepApp2.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public string CourseField { get; set; } = null!;

    public string CourseDescription { get; set; } = null!;

    public string PresenterName { get; set; } = null!;

    public int? SeatCapacity { get; set; }

    public DateOnly CourseDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int? ServiceId { get; set; }

    public int? RequestId { get; set; }

    public virtual Request? Request { get; set; }

    public virtual Service? Service { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
