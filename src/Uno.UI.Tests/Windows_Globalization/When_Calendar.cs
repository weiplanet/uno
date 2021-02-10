﻿#nullable enable
using System;
using System.Globalization;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.UI.Xaml.Media.Animation;
using Windows.Web.Syndication;
using WG = Windows.Globalization;

namespace Uno.UI.Tests.Windows_Globalization
{
	[TestClass]
	public class When_Calendar
	{
		[TestMethod]
		public void When_Gregorian_FixedDate()
		{
			Validate(
				culture: "en-US",
				calendar: WG.CalendarIdentifiers.GregorianValue,
				clock: WG.ClockIdentifiers.TwentyFourHourValue,
				new DateTimeOffset(2020, 01, 02, 03, 04, 05, 0, TimeSpan.Zero),
				year: 2020,
				month: 01,
				day: 02,
				hours: 03,
				minutes: 04,
				seconds: 05,
				milliseconds: 0,
				offsetInSeconds: 0,
				dayOfWeek: DayOfWeek.Thursday,
				era: 1,
				firstDayInThisMonth: 1,
				firstEra: 1,
				firstHourInThisPeriod: 0,
				firstMinuteInThisHour: 0,
				firstMonthInThisYear: 1,
				firstPeriodInThisDay: 1,
				firstSecondInThisMinute: 0,
				firstYearInThisEra: 1,
				isDaylightSavingTime: false,
				lastDayInThisMonth: 31,
				lastEra: 1,
				lastHourInThisPeriod: 23,
				lastMinuteInThisHour: 59,
				lastMonthInThisYear: 12,
				lastPeriodInThisDay: 1,
				lastSecondInThisMinute: 59,
				lastYearInThisEra: 9999,
				numberOfEras: 1,
				numberOfHoursInThisPeriod: 24,
				numberOfMinutesInThisHour: 60,
				numberOfDaysInThisMonth: 31,
				numberOfMonthsInThisYear: 12,
				numberOfPeriodsInThisDay: 1,
				numberOfSecondsInThisMinute: 60,
				numberOfYearsInThisEra: 9998,
				numeralSystem: "",
				period: 1);

			Validate(
				culture: "en-US",
				calendar: WG.CalendarIdentifiers.GregorianValue,
				clock: WG.ClockIdentifiers.TwentyFourHourValue,
				new DateTimeOffset(2020, 08, 02, 03, 04, 05, 00, TimeSpan.Zero),
				year: 2020,
				month: 08,
				day: 02,
				hours: 03,
				minutes: 04,
				seconds: 05,
				milliseconds: 0,
				offsetInSeconds: 0,
				dayOfWeek: DayOfWeek.Sunday,
				era: 1,
				firstDayInThisMonth: 1,
				firstEra: 1,
				firstHourInThisPeriod: 0,
				firstMinuteInThisHour: 0,
				firstMonthInThisYear: 1,
				firstPeriodInThisDay: 1,
				firstSecondInThisMinute: 0,
				firstYearInThisEra: 1,
				isDaylightSavingTime: true,
				lastDayInThisMonth: 31,
				lastEra: 1,
				lastHourInThisPeriod: 23,
				lastMinuteInThisHour: 59,
				lastMonthInThisYear: 12,
				lastPeriodInThisDay: 1,
				lastSecondInThisMinute: 59,
				lastYearInThisEra: 9999,
				numberOfEras: 1,
				numberOfHoursInThisPeriod: 24,
				numberOfMinutesInThisHour: 60,
				numberOfDaysInThisMonth: 31,
				numberOfMonthsInThisYear: 12,
				numberOfPeriodsInThisDay: 1,
				numberOfSecondsInThisMinute: 60,
				numberOfYearsInThisEra: 9998,
				numeralSystem: "",
				period: 1);
		}

		[TestMethod]
		public void When_Gregorian_FixedDate_TwelveHours()
		{
			Validate(
				culture: "en-US",
				calendar: WG.CalendarIdentifiers.GregorianValue,
				clock: WG.ClockIdentifiers.TwelveHour,
				new DateTimeOffset(2020, 01, 02, 23, 04, 05, 00, TimeSpan.Zero),
				year: 2020,
				month: 01,
				day: 02,
				hours: 11,
				minutes: 04,
				seconds: 05,
				milliseconds: 0,
				offsetInSeconds: 0,
				dayOfWeek: DayOfWeek.Thursday,
				era: 1,
				firstDayInThisMonth: 1,
				firstEra: 1,
				firstHourInThisPeriod: 12,
				firstMinuteInThisHour: 0,
				firstMonthInThisYear: 1,
				firstPeriodInThisDay: 1,
				firstSecondInThisMinute: 0,
				firstYearInThisEra: 1,
				isDaylightSavingTime: false,
				lastDayInThisMonth: 31,
				lastEra: 1,
				lastHourInThisPeriod: 11,
				lastMinuteInThisHour: 59,
				lastMonthInThisYear: 12,
				lastPeriodInThisDay: 2,
				lastSecondInThisMinute: 59,
				lastYearInThisEra: 9999,
				numberOfEras: 1,
				numberOfHoursInThisPeriod: 12,
				numberOfMinutesInThisHour: 60,
				numberOfDaysInThisMonth: 31,
				numberOfMonthsInThisYear: 12,
				numberOfPeriodsInThisDay: 2,
				numberOfSecondsInThisMinute: 60,
				numberOfYearsInThisEra: 9998,
				numeralSystem: "",
				period: 2);

			Validate(
				culture: "en-US",
				calendar: WG.CalendarIdentifiers.GregorianValue,
				clock: WG.ClockIdentifiers.TwelveHour,
				new DateTimeOffset(2020, 08, 02, 23, 04, 05, 00, TimeSpan.Zero),
				year: 2020,
				month: 08,
				day: 02,
				hours: 11,
				minutes: 04,
				seconds: 05,
				milliseconds: 0,
				offsetInSeconds: 0,
				dayOfWeek: DayOfWeek.Sunday,
				era: 1,
				firstDayInThisMonth: 1,
				firstEra: 1,
				firstHourInThisPeriod: 12,
				firstMinuteInThisHour: 0,
				firstMonthInThisYear: 1,
				firstPeriodInThisDay: 1,
				firstSecondInThisMinute: 0,
				firstYearInThisEra: 1,
				isDaylightSavingTime: true,
				lastDayInThisMonth: 31,
				lastEra: 1,
				lastHourInThisPeriod: 11,
				lastMinuteInThisHour: 59,
				lastMonthInThisYear: 12,
				lastPeriodInThisDay: 2,
				lastSecondInThisMinute: 59,
				lastYearInThisEra: 9999,
				numberOfEras: 1,
				numberOfHoursInThisPeriod: 12,
				numberOfMinutesInThisHour: 60,
				numberOfDaysInThisMonth: 31,
				numberOfMonthsInThisYear: 12,
				numberOfPeriodsInThisDay: 2,
				numberOfSecondsInThisMinute: 60,
				numberOfYearsInThisEra: 9998,
				numeralSystem: "",
				period: 2);
		}

		[TestMethod]
		public void When_Gregorian_FixedDate_AddSecond()
		{
			Validate(
				culture: "en-US",
				calendar: WG.CalendarIdentifiers.GregorianValue,
				clock: WG.ClockIdentifiers.TwelveHour,
				new DateTimeOffset(2020, 01, 02, 23, 04, 05, 00, TimeSpan.Zero),
				year: 2020,
				month: 01,
				day: 02,
				hours: 11,
				minutes: 04,
				seconds: 06,
				milliseconds: 0,
				offsetInSeconds: 0,
				dayOfWeek: DayOfWeek.Thursday,
				era: 1,
				firstDayInThisMonth: 1,
				firstEra: 1,
				firstHourInThisPeriod: 12,
				firstMinuteInThisHour: 0,
				firstMonthInThisYear: 1,
				firstPeriodInThisDay: 1,
				firstSecondInThisMinute: 0,
				firstYearInThisEra: 1,
				isDaylightSavingTime: false,
				lastDayInThisMonth: 31,
				lastEra: 1,
				lastHourInThisPeriod: 11,
				lastMinuteInThisHour: 59,
				lastMonthInThisYear: 12,
				lastPeriodInThisDay: 2,
				lastSecondInThisMinute: 59,
				lastYearInThisEra: 9999,
				numberOfEras: 1,
				numberOfHoursInThisPeriod: 12,
				numberOfMinutesInThisHour: 60,
				numberOfDaysInThisMonth: 31,
				numberOfMonthsInThisYear: 12,
				numberOfPeriodsInThisDay: 2,
				numberOfSecondsInThisMinute: 60,
				numberOfYearsInThisEra: 9998,
				numeralSystem: "",
				period: 2,
				c => c.AddSeconds(1)
			);
		}

		[TestMethod]
		public void When_Gregorian_FixedDate_AddMinute()
		{
			Validate(
				culture: "en-US",
				calendar: WG.CalendarIdentifiers.GregorianValue,
				clock: WG.ClockIdentifiers.TwelveHour,
				new DateTimeOffset(2020, 01, 02, 23, 04, 05, 00, TimeSpan.Zero),
				year: 2020,
				month: 01,
				day: 02,
				hours: 11,
				minutes: 05,
				seconds: 05,
				milliseconds: 0,
				offsetInSeconds: 0,
				dayOfWeek: DayOfWeek.Thursday,
				era: 1,
				firstDayInThisMonth: 1,
				firstEra: 1,
				firstHourInThisPeriod: 12,
				firstMinuteInThisHour: 0,
				firstMonthInThisYear: 1,
				firstPeriodInThisDay: 1,
				firstSecondInThisMinute: 0,
				firstYearInThisEra: 1,
				isDaylightSavingTime: false,
				lastDayInThisMonth: 31,
				lastEra: 1,
				lastHourInThisPeriod: 11,
				lastMinuteInThisHour: 59,
				lastMonthInThisYear: 12,
				lastPeriodInThisDay: 2,
				lastSecondInThisMinute: 59,
				lastYearInThisEra: 9999,
				numberOfEras: 1,
				numberOfHoursInThisPeriod: 12,
				numberOfMinutesInThisHour: 60,
				numberOfDaysInThisMonth: 31,
				numberOfMonthsInThisYear: 12,
				numberOfPeriodsInThisDay: 2,
				numberOfSecondsInThisMinute: 60,
				numberOfYearsInThisEra: 9998,
				numeralSystem: "",
				period: 2,
				c => c.AddMinutes(1)
			);
		}

		[TestMethod]
		public void When_Gregorian_FixedDate_AddHour()
		{
			Validate(
				culture: "en-US",
				calendar: WG.CalendarIdentifiers.GregorianValue,
				clock: WG.ClockIdentifiers.TwelveHour,
				new DateTimeOffset(2020, 01, 02, 23, 04, 05, 00, TimeSpan.Zero),
				year: 2020,
				month: 01,
				day: 03,
				hours: 00,
				minutes: 04,
				seconds: 05,
				milliseconds: 0,
				offsetInSeconds: 0,
				dayOfWeek: DayOfWeek.Friday,
				era: 1,
				firstDayInThisMonth: 1,
				firstEra: 1,
				firstHourInThisPeriod: 12,
				firstMinuteInThisHour: 0,
				firstMonthInThisYear: 1,
				firstPeriodInThisDay: 1,
				firstSecondInThisMinute: 0,
				firstYearInThisEra: 1,
				isDaylightSavingTime: false,
				lastDayInThisMonth: 31,
				lastEra: 1,
				lastHourInThisPeriod: 11,
				lastMinuteInThisHour: 59,
				lastMonthInThisYear: 12,
				lastPeriodInThisDay: 2,
				lastSecondInThisMinute: 59,
				lastYearInThisEra: 9999,
				numberOfEras: 1,
				numberOfHoursInThisPeriod: 12,
				numberOfMinutesInThisHour: 60,
				numberOfDaysInThisMonth: 31,
				numberOfMonthsInThisYear: 12,
				numberOfPeriodsInThisDay: 2,
				numberOfSecondsInThisMinute: 60,
				numberOfYearsInThisEra: 9998,
				numeralSystem: "",
				period: 1,
				c => c.AddHours(1)
			);
		}

		[TestMethod]
		public void When_Gregorian_FixedDate_AddDay()
		{
			Validate(
				culture: "en-US",
				calendar: WG.CalendarIdentifiers.GregorianValue,
				clock: WG.ClockIdentifiers.TwelveHour,
				new DateTimeOffset(2020, 01, 02, 23, 04, 05, 00, TimeSpan.Zero),
				year: 2020,
				month: 01,
				day: 03,
				hours: 11,
				minutes: 04,
				seconds: 05,
				milliseconds: 0,
				offsetInSeconds: 0,
				dayOfWeek: DayOfWeek.Friday,
				era: 1,
				firstDayInThisMonth: 1,
				firstEra: 1,
				firstHourInThisPeriod: 12,
				firstMinuteInThisHour: 0,
				firstMonthInThisYear: 1,
				firstPeriodInThisDay: 1,
				firstSecondInThisMinute: 0,
				firstYearInThisEra: 1,
				isDaylightSavingTime: false,
				lastDayInThisMonth: 31,
				lastEra: 1,
				lastHourInThisPeriod: 11,
				lastMinuteInThisHour: 59,
				lastMonthInThisYear: 12,
				lastPeriodInThisDay: 2,
				lastSecondInThisMinute: 59,
				lastYearInThisEra: 9999,
				numberOfEras: 1,
				numberOfHoursInThisPeriod: 12,
				numberOfMinutesInThisHour: 60,
				numberOfDaysInThisMonth: 31,
				numberOfMonthsInThisYear: 12,
				numberOfPeriodsInThisDay: 2,
				numberOfSecondsInThisMinute: 60,
				numberOfYearsInThisEra: 9998,
				numeralSystem: "",
				period: 2,
				c => c.AddDays(1)
			);
		}

		[TestMethod]
		public void When_Gregorian_FixedDate_AddMonth()
		{
			Validate(
				culture: "en-US",
				calendar: WG.CalendarIdentifiers.GregorianValue,
				clock: WG.ClockIdentifiers.TwelveHour,
				new DateTimeOffset(2020, 01, 02, 23, 04, 05, 00, TimeSpan.Zero),
				year: 2020,
				month: 02,
				day: 02,
				hours: 11,
				minutes: 04,
				seconds: 05,
				milliseconds: 0,
				offsetInSeconds: 0,
				dayOfWeek: DayOfWeek.Sunday,
				era: 1,
				firstDayInThisMonth: 1,
				firstEra: 1,
				firstHourInThisPeriod: 12,
				firstMinuteInThisHour: 0,
				firstMonthInThisYear: 1,
				firstPeriodInThisDay: 1,
				firstSecondInThisMinute: 0,
				firstYearInThisEra: 1,
				isDaylightSavingTime: false,
				lastDayInThisMonth: 29,
				lastEra: 1,
				lastHourInThisPeriod: 11,
				lastMinuteInThisHour: 59,
				lastMonthInThisYear: 12,
				lastPeriodInThisDay: 2,
				lastSecondInThisMinute: 59,
				lastYearInThisEra: 9999,
				numberOfEras: 1,
				numberOfHoursInThisPeriod: 12,
				numberOfMinutesInThisHour: 60,
				numberOfDaysInThisMonth: 29,
				numberOfMonthsInThisYear: 12,
				numberOfPeriodsInThisDay: 2,
				numberOfSecondsInThisMinute: 60,
				numberOfYearsInThisEra: 9998,
				numeralSystem: "",
				period: 2,
				c => c.AddMonths(1)
			);
		}

		[TestMethod]
		public void When_Gregorian_FixedDate_AddYear()
		{
			Validate(
				culture: "en-US",
				calendar: WG.CalendarIdentifiers.GregorianValue,
				clock: WG.ClockIdentifiers.TwelveHour,
				new DateTimeOffset(2020, 01, 02, 23, 04, 05, 00, TimeSpan.Zero),
				year: 2021,
				month: 01,
				day: 02,
				hours: 11,
				minutes: 04,
				seconds: 05,
				milliseconds: 0,
				offsetInSeconds: 0,
				dayOfWeek: DayOfWeek.Saturday,
				era: 1,
				firstDayInThisMonth: 1,
				firstEra: 1,
				firstHourInThisPeriod: 12,
				firstMinuteInThisHour: 0,
				firstMonthInThisYear: 1,
				firstPeriodInThisDay: 1,
				firstSecondInThisMinute: 0,
				firstYearInThisEra: 1,
				isDaylightSavingTime: false,
				lastDayInThisMonth: 31,
				lastEra: 1,
				lastHourInThisPeriod: 11,
				lastMinuteInThisHour: 59,
				lastMonthInThisYear: 12,
				lastPeriodInThisDay: 2,
				lastSecondInThisMinute: 59,
				lastYearInThisEra: 9999,
				numberOfEras: 1,
				numberOfHoursInThisPeriod: 12,
				numberOfMinutesInThisHour: 60,
				numberOfDaysInThisMonth: 31,
				numberOfMonthsInThisYear: 12,
				numberOfPeriodsInThisDay: 2,
				numberOfSecondsInThisMinute: 60,
				numberOfYearsInThisEra: 9998,
				numeralSystem: "",
				period: 2,
				c => c.AddYears(1)
			);
		}

		[TestMethod]
		[DataRow(2020, "Thursday")]
		[DataRow(2021, "Saturday")]
		public void When_Gregorian_FixedDate_Format_12h(int year, string dayOfWeek)
		{
			ValidateFormat(culture: "en-US",
				  calendar: WG.CalendarIdentifiers.GregorianValue,
				  clock: WG.ClockIdentifiers.TwelveHour,
				  date: new DateTimeOffset(year, 01, 02, 23, 04, 05, 00, TimeSpan.Zero),
				  yearAsPaddedString: year.ToString(CultureInfo.InvariantCulture),
				  yearAsString: year.ToString(CultureInfo.InvariantCulture),
				  monthAsPaddedNumericString: "01",
				  monthAsSoloString: "Jan",
				  monthAsString: "Jan",
				  monthAsNumericString: "1",
				  dayAsPaddedString: "02",
				  dayAsString: "2",
				  hourAsPaddedString: "11",
				  hourAsString: "11",
				  minuteAsPaddedString: "04",
				  minuteAsString: "4",
				  secondAsPaddedString: "05",
				  secondAsString: "5",
				  nanosecondAsPaddedString: "00",
				  nanosecondAsString: "0",
				  dayOfWeekAsSoloString: dayOfWeek,
				  dayOfWeekAsString: dayOfWeek);
		}

		[TestMethod]
		[DataRow(2020, "Thursday")]
		[DataRow(2021, "Saturday")]
		public void When_Gregorian_FixedDate_Format_24h(int year, string dayOfWeek)
		{
			ValidateFormat(culture: "en-US",
				  calendar: WG.CalendarIdentifiers.GregorianValue,
				  clock: WG.ClockIdentifiers.TwentyFourHourValue,
				  date: new DateTimeOffset(year, 01, 02, 23, 04, 05, 00, TimeSpan.Zero),
				  yearAsPaddedString: year.ToString(CultureInfo.InvariantCulture),
				  yearAsString: year.ToString(CultureInfo.InvariantCulture),
				  monthAsPaddedNumericString: "01",
				  monthAsSoloString: "Jan",
				  monthAsString: "Jan",
				  monthAsNumericString: "1",
				  dayAsPaddedString: "02",
				  dayAsString: "2",
				  hourAsPaddedString: "23",
				  hourAsString: "23",
				  minuteAsPaddedString: "04",
				  minuteAsString: "4",
				  secondAsPaddedString: "05",
				  secondAsString: "5",
				  nanosecondAsPaddedString: "00",
				  nanosecondAsString: "0",
				  dayOfWeekAsSoloString: dayOfWeek,
				  dayOfWeekAsString: dayOfWeek);
		}

		private void Validate(
			string culture,
			string calendar,
			string clock,
			DateTimeOffset date,
			int year,
			int month,
			int day,
			int hours,
			int minutes,
			int seconds,
			int milliseconds,
			int offsetInSeconds,
			DayOfWeek dayOfWeek,
			int era,
			int firstDayInThisMonth,
			int firstEra,
			int firstHourInThisPeriod,
			int firstMinuteInThisHour,
			int firstMonthInThisYear,
			int firstPeriodInThisDay,
			int firstSecondInThisMinute,
			int firstYearInThisEra,
			bool isDaylightSavingTime,
			int lastDayInThisMonth,
			int lastEra,
			int lastHourInThisPeriod,
			int lastMinuteInThisHour,
			int lastMonthInThisYear,
			int lastPeriodInThisDay,
			int lastSecondInThisMinute,
			int lastYearInThisEra,
			int numberOfEras,
			int numberOfHoursInThisPeriod,
			int numberOfMinutesInThisHour,
			int numberOfDaysInThisMonth,
			int numberOfMonthsInThisYear,
			int numberOfPeriodsInThisDay,
			int numberOfSecondsInThisMinute,
			int numberOfYearsInThisEra,
			string numeralSystem,
			int period,
			Action<WG.Calendar>? mutator = null
		)
		{
			var SUT = new WG.Calendar(new[] { culture }, calendar, clock);

			SUT.SetDateTime(date);

			mutator?.Invoke(SUT);

			using (new AssertionScope("Calendar Properties"))
			{
				SUT.Day.Should().Be(day, "day");
				SUT.Month.Should().Be(month, "month");
				SUT.Year.Should().Be(year, "year");
				SUT.Hour.Should().Be(hours, "hours");
				SUT.Minute.Should().Be(minutes, "minutes");
				SUT.Second.Should().Be(seconds, "seconds");
				SUT.Nanosecond.Should().Be(milliseconds * 1000, "milliseconds");
				SUT.DayOfWeek.Should().Be(dayOfWeek, "dayOfWeek");
				SUT.Era.Should().Be(era, "era");
				SUT.FirstDayInThisMonth.Should().Be(firstDayInThisMonth, "firstDayInThisMonth");
				SUT.FirstEra.Should().Be(firstEra, "firstEra");
				SUT.FirstHourInThisPeriod.Should().Be(firstHourInThisPeriod, "firstHourInThisPeriod");
				SUT.FirstMinuteInThisHour.Should().Be(firstMinuteInThisHour, "firstMinuteInThisHour");
				SUT.FirstMonthInThisYear.Should().Be(firstMonthInThisYear, "firstMonthInThisYear");
				SUT.FirstPeriodInThisDay.Should().Be(firstPeriodInThisDay, "firstPeriodInThisDay");
				SUT.FirstSecondInThisMinute.Should().Be(firstSecondInThisMinute, "firstSecondInThisMinute");
				SUT.FirstYearInThisEra.Should().Be(firstYearInThisEra, "firstYearInThisEra");
				SUT.Languages.Should().HaveCount(1, "languages count");
				SUT.Languages.Should().HaveElementAt(0, culture, "culture");
				SUT.LastDayInThisMonth.Should().Be(lastDayInThisMonth, "lastDayInThisMonth");
				SUT.LastEra.Should().Be(lastEra, "lastEra");
				SUT.LastHourInThisPeriod.Should().Be(lastHourInThisPeriod, "lastHourInThisPeriod");
				SUT.LastMinuteInThisHour.Should().Be(lastMinuteInThisHour, "lastMinuteInThisHour");
				SUT.LastMonthInThisYear.Should().Be(lastMonthInThisYear, "lastMonthInThisYear");
				SUT.LastPeriodInThisDay.Should().Be(lastPeriodInThisDay, "lastPeriodInThisDay");
				SUT.LastSecondInThisMinute.Should().Be(lastSecondInThisMinute, "lastSecondInThisMinute");
				SUT.LastYearInThisEra.Should().Be(lastYearInThisEra, "lastYearInThisEra");
				SUT.NumberOfDaysInThisMonth.Should().Be(numberOfDaysInThisMonth, "numberOfDaysInThisMonth");
				SUT.NumberOfEras.Should().Be(numberOfEras, "numberOfEras");
				SUT.NumberOfHoursInThisPeriod.Should().Be(numberOfHoursInThisPeriod, "numberOfHoursInThisPeriod");
				SUT.NumberOfMinutesInThisHour.Should().Be(numberOfMinutesInThisHour, "numberOfMinutesInThisHour");
				SUT.NumberOfMonthsInThisYear.Should().Be(numberOfMonthsInThisYear, "numberOfMonthsInThisYear");
				SUT.NumberOfPeriodsInThisDay.Should().Be(numberOfPeriodsInThisDay, "numberOfPeriodsInThisDay");
				SUT.NumberOfSecondsInThisMinute.Should().Be(numberOfSecondsInThisMinute, "numberOfSecondsInThisMinute");
				SUT.NumberOfYearsInThisEra.Should().Be(numberOfYearsInThisEra, "numberOfYearsInThisEra");
				SUT.Period.Should().Be(period, "period");
				SUT.ResolvedLanguage.Should().Be(culture, "culture");

				// Validation is disabled as timezone support is only using the current machine's timezone
				// SUT.IsDaylightSavingTime.Should().Be(isDaylightSavingTime, "isDaylightSavingTime");

				// Not yet supported.
				// SUT.NumeralSystem.Should().Be(numeralSystem, "numeralSystem");
			}
		}

		private void ValidateFormat(
			string culture,
			string calendar,
			string clock,
			DateTimeOffset date,
			string yearAsPaddedString,
			string yearAsString,
			string monthAsPaddedNumericString,
			string monthAsSoloString,
			string monthAsString,
			string monthAsNumericString,
			string dayAsPaddedString,
			string dayAsString,
			string hourAsPaddedString,
			string hourAsString,
			string minuteAsPaddedString,
			string minuteAsString,
			string secondAsPaddedString,
			string secondAsString,
			string nanosecondAsPaddedString,
			string nanosecondAsString,
			string dayOfWeekAsSoloString,
			string dayOfWeekAsString
			)
		{
			var SUT = new WG.Calendar(new[] { culture }, calendar, clock);

			SUT.SetDateTime(date);

			using (new AssertionScope("Calendar Format"))
			{
				SUT.YearAsPaddedString(2).Should().Be(yearAsPaddedString, "yearAsPaddedString");
				SUT.YearAsString().Should().Be(yearAsString, "yearAsString");
				SUT.MonthAsPaddedNumericString(2).Should().Be(monthAsPaddedNumericString, "monthAsPaddedNumericString");
				SUT.MonthAsSoloString().Should().Be(monthAsSoloString, "monthAsSoloString");
				SUT.MonthAsString().Should().Be(monthAsString, "monthAsString");
				SUT.MonthAsNumericString().Should().Be(monthAsNumericString, "monthAsNumericString");
				SUT.DayAsPaddedString(2).Should().Be(dayAsPaddedString, "dayAsPaddedString");
				SUT.DayAsString().Should().Be(dayAsString, "dayAsString");
				SUT.HourAsPaddedString(2).Should().Be(hourAsPaddedString, "hourAsPaddedString");
				SUT.HourAsString().Should().Be(hourAsString, "hourAsString");
				SUT.MinuteAsPaddedString(2).Should().Be(minuteAsPaddedString, "minuteAsPaddedString");
				SUT.MinuteAsString().Should().Be(minuteAsString, "minuteAsString");
				SUT.SecondAsPaddedString(2).Should().Be(secondAsPaddedString, "secondAsPaddedString");
				SUT.SecondAsString().Should().Be(secondAsString, "secondAsString");
				SUT.NanosecondAsPaddedString(2).Should().Be(nanosecondAsPaddedString, "nanosecondAsPaddedString");
				SUT.NanosecondAsString().Should().Be(nanosecondAsString, "nanosecondAsString");
				SUT.DayOfWeekAsSoloString().Should().Be(dayOfWeekAsSoloString, "dayOfWeekAsSoloString");
				SUT.DayOfWeekAsString().Should().Be(dayOfWeekAsString, "dayOfWeekAsString");
			}
		}
	}
}
