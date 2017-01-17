using Xunit;

namespace Microsoft.VisualStudio.Services.Agent.Tests
{
    public sealed class ConditionL0
    {
        ////////////////////////////////////////////////////////////////////////////////
        // Simple conditions
        ////////////////////////////////////////////////////////////////////////////////
        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void EvaluatesBool()
        {
            using (var hc = new TestHostContext(this))
            {
                Assert.Equal(true, new Condition(hc, "true").Result);
                Assert.Equal(true, new Condition(hc, "TRUE").Result);
                Assert.Equal(false, new Condition(hc, "false").Result);
                Assert.Equal(false, new Condition(hc, "FALSE").Result);
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void TreatsNumberAsTruthy()
        {
            using (var hc = new TestHostContext(this))
            {
                Assert.Equal(true, new Condition(hc, "1").Result);
                Assert.Equal(true, new Condition(hc, ".5").Result);
                Assert.Equal(true, new Condition(hc, "0.5").Result);
                Assert.Equal(true, new Condition(hc, "2").Result);
                Assert.Equal(true, new Condition(hc, "-1").Result);
                Assert.Equal(true, new Condition(hc, "-.5").Result);
                Assert.Equal(true, new Condition(hc, "-0.5").Result);
                Assert.Equal(true, new Condition(hc, "-2").Result);
                Assert.Equal(false, new Condition(hc, "0").Result);
                Assert.Equal(false, new Condition(hc, "0.0").Result);
                Assert.Equal(false, new Condition(hc, "-0").Result);
                Assert.Equal(false, new Condition(hc, "-0.0").Result);
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void TreatsStringAsTruthy()
        {
            using (var hc = new TestHostContext(this))
            {
                Assert.Equal(true, new Condition(hc, "'a'").Result);
                Assert.Equal(true, new Condition(hc, "'false'").Result);
                Assert.Equal(true, new Condition(hc, "'0'").Result);
                Assert.Equal(true, new Condition(hc, "' '").Result);
                Assert.Equal(false, new Condition(hc, "''").Result);
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void TreatsVersionAsTruthy()
        {
            using (var hc = new TestHostContext(this))
            {
                Assert.Equal(true, new Condition(hc, "1.2.3").Result);
                Assert.Equal(true, new Condition(hc, "1.2.3.4").Result);
                Assert.Equal(true, new Condition(hc, "0.0.0").Result);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Functions
        ////////////////////////////////////////////////////////////////////////////////
        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void EvaluatesAnd()
        {
            using (var hc = new TestHostContext(this))
            {
                Assert.Equal(true, new Condition(hc, "and(true, true, true)").Result); // bool
                Assert.Equal(true, new Condition(hc, "and(true, true)").Result);
                Assert.Equal(false, new Condition(hc, "and(true, true, false)").Result);
                Assert.Equal(false, new Condition(hc, "and(true, false)").Result);
                Assert.Equal(false, new Condition(hc, "and(false, true)").Result);
                Assert.Equal(false, new Condition(hc, "and(false, false)").Result);
                Assert.Equal(true, new Condition(hc, "and(true, 1)").Result); // number
                Assert.Equal(false, new Condition(hc, "and(true, 0)").Result);
                Assert.Equal(true, new Condition(hc, "and(true, 'a')").Result); // string
                Assert.Equal(false, new Condition(hc, "and(true, '')").Result);
                Assert.Equal(true, new Condition(hc, "and(true, 0.0.0.0)").Result); // version
                Assert.Equal(true, new Condition(hc, "and(true, 1.2.3.4)").Result);
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void AndShortCircuitsAndAfterFirstFalse()
        {
            using (var hc = new TestHostContext(this))
            {
                // The gt function should never evaluate. It would would throw since 'not a number'
                // cannot be converted to a number.
                Assert.Equal(false, new Condition(hc, "and(false, gt(1, 'not a number'))").Result);
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void EvaluatesEqual()
        {
            using (var hc = new TestHostContext(this))
            {
                Assert.Equal(true, new Condition(hc, "eq(true, true)").Result); // bool
                Assert.Equal(true, new Condition(hc, "eq(false, false)").Result);
                Assert.Equal(false, new Condition(hc, "eq(false, true)").Result);
                Assert.Equal(true, new Condition(hc, "eq(2, 2)").Result); // number
                Assert.Equal(false, new Condition(hc, "eq(1, 2)").Result);
                Assert.Equal(true, new Condition(hc, "eq('abcDEF', 'ABCdef')").Result); // string
                Assert.Equal(false, new Condition(hc, "eq('a', 'b')").Result);
                Assert.Equal(true, new Condition(hc, "eq(1.2.3, 1.2.3)").Result); // version
                Assert.Equal(false, new Condition(hc, "eq(1.2.3, 1.2.3.0)").Result);
                Assert.Equal(false, new Condition(hc, "eq(1.2.3, 4.5.6)").Result);
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void EqualCastsToMatchLeftSide()
        {
            using (var hc = new TestHostContext(this))
            {
                // Cast to bool.
                Assert.Equal(true, new Condition(hc, "eq(true, 2)").Result); // number
                Assert.Equal(true, new Condition(hc, "eq(false, 0)").Result);
                Assert.Equal(true, new Condition(hc, "eq(true, 'a')").Result); // string
                Assert.Equal(true, new Condition(hc, "eq(true, ' ')").Result);
                Assert.Equal(true, new Condition(hc, "eq(false, '')").Result);
                Assert.Equal(true, new Condition(hc, "eq(true, 1.2.3)").Result); // version
                Assert.Equal(true, new Condition(hc, "eq(true, 0.0.0)").Result);

                // Cast to string.
                Assert.Equal(true, new Condition(hc, "eq('TRue', true)").Result); // bool
                Assert.Equal(true, new Condition(hc, "eq('FALse', false)").Result);
                Assert.Equal(true, new Condition(hc, "eq('123456.789', 123456.789)").Result); // number
                Assert.Equal(false, new Condition(hc, "eq('123456.000', 123456.000)").Result);
                Assert.Equal(true, new Condition(hc, "eq('1.2.3', 1.2.3)").Result); // version

                // Cast to number (best effort).
                Assert.Equal(true, new Condition(hc, "eq(1, true)").Result); // bool
                Assert.Equal(true, new Condition(hc, "eq(0, false)").Result);
                Assert.Equal(false, new Condition(hc, "eq(2, true)").Result);
                Assert.Equal(true, new Condition(hc, "eq(123456.789, ' +123,456.7890 ')").Result); // string
                Assert.Equal(true, new Condition(hc, "eq(-123456.789, ' -123,456.7890 ')").Result);
                Assert.Equal(true, new Condition(hc, "eq(123000, ' 123,000.000 ')").Result);
                Assert.Equal(false, new Condition(hc, "eq(1, 'not a number')").Result);
                Assert.Equal(false, new Condition(hc, "eq(0, 'not a number')").Result);
                Assert.Equal(false, new Condition(hc, "eq(1.2, 1.2.0.0)").Result); // version

                // Cast to version (best effort).
                Assert.Equal(false, new Condition(hc, "eq(1.2.3, false)").Result); // bool
                Assert.Equal(false, new Condition(hc, "eq(1.2.3, true)").Result);
                Assert.Equal(false, new Condition(hc, "eq(1.2.0, 1.2)").Result); // number
                Assert.Equal(true, new Condition(hc, "eq(1.2.0, ' 1.2.0 ')").Result); // string
                Assert.Equal(false, new Condition(hc, "eq(1.2.0, '1.2')").Result);
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void EvaluatesGreaterThan()
        {
            using (var hc = new TestHostContext(this))
            {
                Assert.Equal(true, new Condition(hc, "gt(true, false)").Result); // bool
                Assert.Equal(false, new Condition(hc, "gt(true, true)").Result);
                Assert.Equal(false, new Condition(hc, "gt(false, true)").Result);
                Assert.Equal(false, new Condition(hc, "gt(false, false)").Result);
                Assert.Equal(true, new Condition(hc, "gt(2, 1)").Result); // number
                Assert.Equal(false, new Condition(hc, "gt(1, 2)").Result);
                Assert.Equal(true, new Condition(hc, "gt('DEF', 'abc')").Result); // string
                Assert.Equal(true, new Condition(hc, "gt('def', 'ABC')").Result);
                Assert.Equal(false, new Condition(hc, "gt('a', 'b')").Result);
                Assert.Equal(true, new Condition(hc, "gt(4.5.6, 1.2.3)").Result); // version
                Assert.Equal(false, new Condition(hc, "gt(1.2.3, 4.5.6)").Result);
                Assert.Equal(false, new Condition(hc, "gt(1.2.3, 1.2.3)").Result);
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void EvaluatesNot()
        {
            using (var hc = new TestHostContext(this))
            {
                Assert.Equal(true, new Condition(hc, "not(false)").Result); // bool
                Assert.Equal(false, new Condition(hc, "not(true)").Result);
                Assert.Equal(true, new Condition(hc, "not(0)").Result); // number
                Assert.Equal(false, new Condition(hc, "not(1)").Result);
                Assert.Equal(true, new Condition(hc, "not('')").Result); // string
                Assert.Equal(false, new Condition(hc, "not('a')").Result);
                Assert.Equal(false, new Condition(hc, "not(' ')").Result);
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void EvaluatesNotEqual()
        {
            using (var hc = new TestHostContext(this))
            {
                Assert.Equal(true, new Condition(hc, "ne(false, true)").Result); // bool
                Assert.Equal(true, new Condition(hc, "ne(true, false)").Result);
                Assert.Equal(false, new Condition(hc, "ne(false, false)").Result);
                Assert.Equal(false, new Condition(hc, "ne(true, true)").Result);
                Assert.Equal(true, new Condition(hc, "ne(1, 2)").Result); // number
                Assert.Equal(false, new Condition(hc, "ne(2, 2)").Result);
                Assert.Equal(true, new Condition(hc, "ne('abc', 'def')").Result); // string
                Assert.Equal(false, new Condition(hc, "ne('abcDEF', 'ABCdef')").Result);
                Assert.Equal(true, new Condition(hc, "ne(1.2.3, 1.2.3.0)").Result); // version
                Assert.Equal(true, new Condition(hc, "ne(1.2.3, 4.5.6)").Result);
                Assert.Equal(false, new Condition(hc, "ne(1.2.3, 1.2.3)").Result);
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void NotEqualCastsToMatchLeftSide()
        {
            using (var hc = new TestHostContext(this))
            {
                // Cast to bool.
                Assert.Equal(true, new Condition(hc, "ne(false, 2)").Result); // number
                Assert.Equal(true, new Condition(hc, "ne(true, 0)").Result);
                Assert.Equal(true, new Condition(hc, "ne(false, 'a')").Result); // string
                Assert.Equal(true, new Condition(hc, "ne(false, ' ')").Result);
                Assert.Equal(true, new Condition(hc, "ne(true, '')").Result);
                Assert.Equal(true, new Condition(hc, "ne(false, 1.2.3)").Result); // version
                Assert.Equal(true, new Condition(hc, "ne(false, 0.0.0)").Result);

                // Cast to string.
                Assert.Equal(false, new Condition(hc, "ne('TRue', true)").Result); // bool
                Assert.Equal(false, new Condition(hc, "ne('FALse', false)").Result);
                Assert.Equal(true, new Condition(hc, "ne('123456.000', 123456.000)").Result); // number
                Assert.Equal(false, new Condition(hc, "ne('123456.789', 123456.789)").Result);
                Assert.Equal(true, new Condition(hc, "ne('1.2.3.0', 1.2.3)").Result); // version
                Assert.Equal(false, new Condition(hc, "ne('1.2.3', 1.2.3)").Result);

                // Cast to number (best effort).
                Assert.Equal(true, new Condition(hc, "ne(2, true)").Result); // bool
                Assert.Equal(false, new Condition(hc, "ne(1, true)").Result);
                Assert.Equal(false, new Condition(hc, "ne(0, false)").Result);
                Assert.Equal(false, new Condition(hc, "ne(123456.789, ' +123,456.7890 ')").Result); // string
                Assert.Equal(false, new Condition(hc, "ne(-123456.789, ' -123,456.7890 ')").Result);
                Assert.Equal(false, new Condition(hc, "ne(123000, ' 123,000.000 ')").Result);
                Assert.Equal(true, new Condition(hc, "ne(1, 'not a number')").Result);
                Assert.Equal(true, new Condition(hc, "ne(0, 'not a number')").Result);
                Assert.Equal(true, new Condition(hc, "ne(1.2, 1.2.0.0)").Result); // version

                // Cast to version (best effort).
                Assert.Equal(true, new Condition(hc, "ne(1.2.3, false)").Result); // bool
                Assert.Equal(true, new Condition(hc, "ne(1.2.3, true)").Result);
                Assert.Equal(true, new Condition(hc, "ne(1.2.0, 1.2)").Result); // number
                Assert.Equal(false, new Condition(hc, "ne(1.2.0, ' 1.2.0 ')").Result); // string
                Assert.Equal(true, new Condition(hc, "ne(1.2.0, '1.2')").Result);
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void EvaluatesOr()
        {
            using (var hc = new TestHostContext(this))
            {
                Assert.Equal(true, new Condition(hc, "or(false, false, true)").Result); // bool
                Assert.Equal(true, new Condition(hc, "or(false, true, false)").Result);
                Assert.Equal(true, new Condition(hc, "or(true, false, false)").Result);
                Assert.Equal(false, new Condition(hc, "or(false, false, false)").Result);
                Assert.Equal(true, new Condition(hc, "or(false, 1)").Result); // number
                Assert.Equal(false, new Condition(hc, "or(false, 0)").Result);
                Assert.Equal(true, new Condition(hc, "or(false, 'a')").Result); // string
                Assert.Equal(false, new Condition(hc, "or(false, '')").Result);
                Assert.Equal(true, new Condition(hc, "or(false, 1.2.3)").Result); // version
                Assert.Equal(true, new Condition(hc, "or(false, 0.0.0)").Result);
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void ShortCircuitsOrAfterFirstTrue()
        {
            using (var hc = new TestHostContext(this))
            {
                // The gt function should never evaluate. It would would throw since 'not a number'
                // cannot be converted to a number.
                Assert.Equal(true, new Condition(hc, "or(true, gt(1, 'not a number'))").Result);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Parse exceptions
        ////////////////////////////////////////////////////////////////////////////////
        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void ThrowsWhenInvalidNumber()
        {
            using (var hc = new TestHostContext(this))
            {
                try
                {
                    new Condition(hc, "eq(1.2, 3.4a)");
                }
                catch (Condition.ParseException ex)
                {
                    Assert.Equal(Condition.ParseExceptionKind.UnrecognizedValue, ex.Kind);
                    Assert.Equal("3.4a", ex.RawToken);
                }
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void ThrowsWhenInvalidVersion()
        {
            using (var hc = new TestHostContext(this))
            {
                try
                {
                    new Condition(hc, "eq(1.2.3, 4.5.6.7a)");
                }
                catch (Condition.ParseException ex)
                {
                    Assert.Equal(Condition.ParseExceptionKind.UnrecognizedValue, ex.Kind);
                    Assert.Equal("4.5.6.7a", ex.RawToken);
                }
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void ThrowsWhenInvalidString()
        {
            using (var hc = new TestHostContext(this))
            {
                try
                {
                    new Condition(hc, "eq('hello', 'unterminated-string)");
                }
                catch (Condition.ParseException ex)
                {
                    Assert.Equal(Condition.ParseExceptionKind.UnrecognizedValue, ex.Kind);
                    Assert.Equal("'unterminated-string)", ex.RawToken);
                }
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void ThrowsWhenUnclosedFunction()
        {
            using (var hc = new TestHostContext(this))
            {
                try
                {
                    new Condition(hc, "eq(1,2");
                }
                catch (Condition.ParseException ex)
                {
                    Assert.Equal(Condition.ParseExceptionKind.UnclosedFunction, ex.Kind);
                    Assert.Equal("eq", ex.RawToken);
                }
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void ThrowsWhenExpectedOpenFunction()
        {
            using (var hc = new TestHostContext(this))
            {
                try
                {
                    new Condition(hc, "not(eq 1,2)");
                }
                catch (Condition.ParseException ex)
                {
                    Assert.Equal(Condition.ParseExceptionKind.ExpectedOpenFunction, ex.Kind);
                    Assert.Equal("eq", ex.RawToken);
                }
            }
        }
    }
}
