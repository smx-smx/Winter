#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System.Xml.Serialization;

namespace Smx.Winter.SchemaDefinitions.TaskNS
{
    [XmlRoot(ElementName = "RegistrationInfo", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class RegistrationInfo
    {
        [XmlElement(ElementName = "URI", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string URI { get; set; }
        [XmlElement(ElementName = "Source", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Source { get; set; }
        [XmlElement(ElementName = "Author", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Author { get; set; }
        [XmlElement(ElementName = "Description", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Description { get; set; }
        [XmlElement(ElementName = "Version", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Version { get; set; }
        [XmlElement(ElementName = "SecurityDescriptor", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string SecurityDescriptor { get; set; }
        [XmlElement(ElementName = "Date", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Date { get; set; }
    }

    [XmlRoot(ElementName = "Repetition", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class Repetition
    {
        [XmlElement(ElementName = "Interval", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Interval { get; set; }
        [XmlElement(ElementName = "StopAtDurationEnd", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string StopAtDurationEnd { get; set; }
        [XmlElement(ElementName = "Duration", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Duration { get; set; }
    }

    [XmlRoot(ElementName = "LogonTrigger", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class LogonTrigger
    {
        [XmlElement(ElementName = "Enabled", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Enabled { get; set; }
        [XmlElement(ElementName = "Delay", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Delay { get; set; }
        [XmlElement(ElementName = "Repetition", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public Repetition Repetition { get; set; }
        [XmlElement(ElementName = "StartBoundary", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string StartBoundary { get; set; }
        [XmlElement(ElementName = "EndBoundary", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string EndBoundary { get; set; }
        [XmlElement(ElementName = "ExecutionTimeLimit", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string ExecutionTimeLimit { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "ScheduleByDay", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class ScheduleByDay
    {
        [XmlElement(ElementName = "DaysInterval", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string DaysInterval { get; set; }
    }

    [XmlRoot(ElementName = "DaysOfMonth", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class DaysOfMonth
    {
        [XmlElement(ElementName = "Day", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public List<string> Day { get; set; }
    }

    [XmlRoot(ElementName = "Months", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class Months
    {
        [XmlElement(ElementName = "January", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string January { get; set; }
        [XmlElement(ElementName = "February", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string February { get; set; }
        [XmlElement(ElementName = "March", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string March { get; set; }
        [XmlElement(ElementName = "April", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string April { get; set; }
        [XmlElement(ElementName = "May", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string May { get; set; }
        [XmlElement(ElementName = "June", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string June { get; set; }
        [XmlElement(ElementName = "July", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string July { get; set; }
        [XmlElement(ElementName = "August", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string August { get; set; }
        [XmlElement(ElementName = "September", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string September { get; set; }
        [XmlElement(ElementName = "October", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string October { get; set; }
        [XmlElement(ElementName = "November", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string November { get; set; }
        [XmlElement(ElementName = "December", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string December { get; set; }
    }

    [XmlRoot(ElementName = "ScheduleByMonth", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class ScheduleByMonth
    {
        [XmlElement(ElementName = "DaysOfMonth", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public DaysOfMonth DaysOfMonth { get; set; }
        [XmlElement(ElementName = "Months", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public Months Months { get; set; }
    }

    [XmlRoot(ElementName = "CalendarTrigger", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class CalendarTrigger
    {
        [XmlElement(ElementName = "StartBoundary", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string StartBoundary { get; set; }
        [XmlElement(ElementName = "Enabled", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Enabled { get; set; }
        [XmlElement(ElementName = "ScheduleByDay", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public ScheduleByDay ScheduleByDay { get; set; }
        [XmlElement(ElementName = "RandomDelay", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string RandomDelay { get; set; }
        [XmlElement(ElementName = "ScheduleByMonth", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public ScheduleByMonth ScheduleByMonth { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "BootTrigger", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class BootTrigger
    {
        [XmlElement(ElementName = "Enabled", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Enabled { get; set; }
        [XmlElement(ElementName = "Delay", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Delay { get; set; }
        [XmlElement(ElementName = "Repetition", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public Repetition Repetition { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "EndBoundary", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string EndBoundary { get; set; }
    }

    [XmlRoot(ElementName = "WnfStateChangeTrigger", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class WnfStateChangeTrigger
    {
        [XmlElement(ElementName = "StateName", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string StateName { get; set; }
        [XmlElement(ElementName = "Data", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Data { get; set; }
        [XmlElement(ElementName = "Enabled", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Enabled { get; set; }
        [XmlElement(ElementName = "Delay", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Delay { get; set; }
        [XmlElement(ElementName = "DataOffset", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string DataOffset { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "TimeTrigger", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class TimeTrigger
    {
        [XmlElement(ElementName = "Enabled", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Enabled { get; set; }
        [XmlElement(ElementName = "StartBoundary", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string StartBoundary { get; set; }
        [XmlElement(ElementName = "EndBoundary", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string EndBoundary { get; set; }
        [XmlElement(ElementName = "Repetition", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public Repetition Repetition { get; set; }
        [XmlElement(ElementName = "RandomDelay", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string RandomDelay { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "RegistrationTrigger", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class RegistrationTrigger
    {
        [XmlElement(ElementName = "Enabled", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Enabled { get; set; }
        [XmlElement(ElementName = "Delay", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Delay { get; set; }
    }

    [XmlRoot(ElementName = "EventTrigger", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class EventTrigger
    {
        [XmlElement(ElementName = "Enabled", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Enabled { get; set; }
        [XmlElement(ElementName = "Subscription", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Subscription { get; set; }
        [XmlElement(ElementName = "Delay", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Delay { get; set; }
        [XmlElement(ElementName = "Repetition", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public Repetition Repetition { get; set; }
        [XmlElement(ElementName = "ExecutionTimeLimit", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string ExecutionTimeLimit { get; set; }
        [XmlElement(ElementName = "StartBoundary", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string StartBoundary { get; set; }
        [XmlElement(ElementName = "EndBoundary", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string EndBoundary { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "SessionStateChangeTrigger", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class SessionStateChangeTrigger
    {
        [XmlElement(ElementName = "StateChange", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string StateChange { get; set; }
        [XmlElement(ElementName = "Enabled", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Enabled { get; set; }
        [XmlElement(ElementName = "Delay", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Delay { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "IdleTrigger", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class IdleTrigger
    {
        [XmlElement(ElementName = "Enabled", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Enabled { get; set; }
        [XmlElement(ElementName = "Repetition", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public Repetition Repetition { get; set; }
        [XmlElement(ElementName = "StartBoundary", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string StartBoundary { get; set; }
        [XmlElement(ElementName = "EndBoundary", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string EndBoundary { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "Triggers", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class Triggers
    {
        [XmlElement(ElementName = "LogonTrigger", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public LogonTrigger LogonTrigger { get; set; }
        [XmlElement(ElementName = "CalendarTrigger", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public CalendarTrigger CalendarTrigger { get; set; }
        [XmlElement(ElementName = "BootTrigger", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public BootTrigger BootTrigger { get; set; }
        [XmlElement(ElementName = "WnfStateChangeTrigger", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public List<WnfStateChangeTrigger> WnfStateChangeTrigger { get; set; }
        [XmlElement(ElementName = "TimeTrigger", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public List<TimeTrigger> TimeTrigger { get; set; }
        [XmlElement(ElementName = "RegistrationTrigger", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public RegistrationTrigger RegistrationTrigger { get; set; }
        [XmlElement(ElementName = "EventTrigger", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public List<EventTrigger> EventTrigger { get; set; }
        [XmlElement(ElementName = "SessionStateChangeTrigger", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public List<SessionStateChangeTrigger> SessionStateChangeTrigger { get; set; }
        [XmlElement(ElementName = "IdleTrigger", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public IdleTrigger IdleTrigger { get; set; }
    }

    [XmlRoot(ElementName = "MaintenanceSettings", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class MaintenanceSettings
    {
        [XmlElement(ElementName = "Period", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Period { get; set; }
        [XmlElement(ElementName = "Deadline", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Deadline { get; set; }
        [XmlElement(ElementName = "Exclusive", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Exclusive { get; set; }
    }

    [XmlRoot(ElementName = "IdleSettings", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class IdleSettings
    {
        [XmlElement(ElementName = "StopOnIdleEnd", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string StopOnIdleEnd { get; set; }
        [XmlElement(ElementName = "RestartOnIdle", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string RestartOnIdle { get; set; }
        [XmlElement(ElementName = "Duration", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Duration { get; set; }
        [XmlElement(ElementName = "WaitTimeout", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string WaitTimeout { get; set; }
    }

    [XmlRoot(ElementName = "RestartOnFailure", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class RestartOnFailure
    {
        [XmlElement(ElementName = "Interval", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Interval { get; set; }
        [XmlElement(ElementName = "Count", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Count { get; set; }
    }

    [XmlRoot(ElementName = "Settings", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class Settings
    {
        [XmlElement(ElementName = "MultipleInstancesPolicy", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string MultipleInstancesPolicy { get; set; }
        [XmlElement(ElementName = "DisallowStartIfOnBatteries", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string DisallowStartIfOnBatteries { get; set; }
        [XmlElement(ElementName = "StopIfGoingOnBatteries", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string StopIfGoingOnBatteries { get; set; }
        [XmlElement(ElementName = "AllowHardTerminate", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string AllowHardTerminate { get; set; }
        [XmlElement(ElementName = "StartWhenAvailable", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string StartWhenAvailable { get; set; }
        [XmlElement(ElementName = "RunOnlyIfNetworkAvailable", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string RunOnlyIfNetworkAvailable { get; set; }
        [XmlElement(ElementName = "MaintenanceSettings", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public MaintenanceSettings MaintenanceSettings { get; set; }
        [XmlElement(ElementName = "AllowStartOnDemand", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string AllowStartOnDemand { get; set; }
        [XmlElement(ElementName = "Enabled", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Enabled { get; set; }
        [XmlElement(ElementName = "Hidden", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Hidden { get; set; }
        [XmlElement(ElementName = "RunOnlyIfIdle", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string RunOnlyIfIdle { get; set; }
        [XmlElement(ElementName = "DisallowStartOnRemoteAppSession", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string DisallowStartOnRemoteAppSession { get; set; }
        [XmlElement(ElementName = "UseUnifiedSchedulingEngine", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string UseUnifiedSchedulingEngine { get; set; }
        [XmlElement(ElementName = "WakeToRun", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string WakeToRun { get; set; }
        [XmlElement(ElementName = "Priority", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Priority { get; set; }
        [XmlElement(ElementName = "ExecutionTimeLimit", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string ExecutionTimeLimit { get; set; }
        [XmlElement(ElementName = "IdleSettings", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public IdleSettings IdleSettings { get; set; }
        [XmlElement(ElementName = "DeleteExpiredTaskAfter", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string DeleteExpiredTaskAfter { get; set; }
        [XmlElement(ElementName = "RestartOnFailure", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public RestartOnFailure RestartOnFailure { get; set; }
    }

    [XmlRoot(ElementName = "RequiredPrivileges", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class RequiredPrivileges
    {
        [XmlElement(ElementName = "Privilege", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Privilege { get; set; }
    }

    [XmlRoot(ElementName = "Principal", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class Principal
    {
        [XmlElement(ElementName = "UserId", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string UserId { get; set; }
        [XmlElement(ElementName = "GroupId", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string GroupId { get; set; }
        [XmlElement(ElementName = "RunLevel", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string RunLevel { get; set; }
        [XmlElement(ElementName = "RequiredPrivileges", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public RequiredPrivileges RequiredPrivileges { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "Principals", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class Principals
    {
        [XmlElement(ElementName = "Principal", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public Principal Principal { get; set; }
    }

    [XmlRoot(ElementName = "ComHandler", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class ComHandler
    {
        [XmlElement(ElementName = "ClassId", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string ClassId { get; set; }
        [XmlElement(ElementName = "Data", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Data { get; set; }
    }

    [XmlRoot(ElementName = "Exec", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class Exec
    {
        [XmlElement(ElementName = "Command", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Command { get; set; }
        [XmlElement(ElementName = "Arguments", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string Arguments { get; set; }
        [XmlElement(ElementName = "WorkingDirectory", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public string WorkingDirectory { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "Actions", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class Actions
    {
        [XmlElement(ElementName = "ComHandler", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public ComHandler ComHandler { get; set; }
        [XmlElement(ElementName = "Exec", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public List<Exec> Exec { get; set; }
        [XmlAttribute(AttributeName = "Context")]
        public string Context { get; set; }
    }

    [XmlRoot(ElementName = "Task", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
    public class Task
    {
        [XmlElement(ElementName = "RegistrationInfo", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public RegistrationInfo RegistrationInfo { get; set; }
        [XmlElement(ElementName = "Triggers", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public Triggers Triggers { get; set; }
        [XmlElement(ElementName = "Settings", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public Settings Settings { get; set; }
        [XmlElement(ElementName = "Principals", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public Principals Principals { get; set; }
        [XmlElement(ElementName = "Actions", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public Actions Actions { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
    }
}
