using System;
using System.Collections.Generic;
using System.Transactions;
using FluentAssertions;
using Xunit;

namespace Core.Tests;

public class TransactionScopeTests
{
    [Theory]
    [InlineData(EnlistmentOptions.None)]
    [InlineData(EnlistmentOptions.EnlistDuringPrepareRequired)]
    public void WhenScopeCompletedAndPrepared_ThenPrepareAndCommitCalled(EnlistmentOptions enlistmentOptions)
    {
        Transaction.Current.Should().BeNull();
        TestEnlistmentNotification enlistment;

        using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
        {
            enlistment = Enlist(PrepareAction.Prepared, enlistmentOptions);
            enlistment.Notifications.Should().BeEquivalentTo(new List<Notification>());

            Assert(enlistment);

            scope.Complete();

            Assert(enlistment);
        }

        Assert(enlistment, Notification.Prepare, Notification.Commit);
        Transaction.Current.Should().BeNull();
    }

    [Theory]
    [InlineData(EnlistmentOptions.None)]
    [InlineData(EnlistmentOptions.EnlistDuringPrepareRequired)]
    public void WhenScopeCompletedAndDone_ThenPrepareOnlyCalled(EnlistmentOptions enlistmentOptions)
    {
        Transaction.Current.Should().BeNull();
        TestEnlistmentNotification enlistment;

        using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
        {
            enlistment = Enlist(PrepareAction.Done, enlistmentOptions);
            enlistment.Notifications.Should().BeEquivalentTo(new List<Notification>());

            Assert(enlistment);

            scope.Complete();

            Assert(enlistment);
        }

        Assert(enlistment, Notification.Prepare);
        Transaction.Current.Should().BeNull();
    }

    [Theory]
    [InlineData(EnlistmentOptions.None)]
    [InlineData(EnlistmentOptions.EnlistDuringPrepareRequired)]
    public void WhenScopeCompletedAndForceRollback_ThenPrepareOnlyCalledAndTransactionAbortedThrown(EnlistmentOptions enlistmentOptions)
    {
        Transaction.Current.Should().BeNull();
        TestEnlistmentNotification enlistment = null!;

        Xunit.Assert.Throws<TransactionAbortedException>(() =>
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                enlistment = Enlist(PrepareAction.ForceRollback, enlistmentOptions);
                enlistment.Notifications.Should().BeEquivalentTo(new List<Notification>());

                Assert(enlistment);

                scope.Complete();

                Assert(enlistment);
            }
        });

        Assert(enlistment, Notification.Prepare);
        Transaction.Current.Should().BeNull();
    }

    [Theory]
    [InlineData(PrepareAction.Prepared, EnlistmentOptions.None)]
    [InlineData(PrepareAction.Done, EnlistmentOptions.None)]
    [InlineData(PrepareAction.ForceRollback, EnlistmentOptions.None)]
    [InlineData(PrepareAction.Prepared, EnlistmentOptions.EnlistDuringPrepareRequired)]
    [InlineData(PrepareAction.Done, EnlistmentOptions.EnlistDuringPrepareRequired)]
    [InlineData(PrepareAction.ForceRollback, EnlistmentOptions.EnlistDuringPrepareRequired)]
    public void WhenScopeDisposed_ThenRollbackCalled(PrepareAction prepareAction, EnlistmentOptions enlistmentOptions)
    {
        Transaction.Current.Should().BeNull();
        TestEnlistmentNotification enlistment;

        using (new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
        {
            enlistment = Enlist(prepareAction, enlistmentOptions);
            enlistment.Notifications.Should().BeEquivalentTo(new List<Notification>());

            Assert(enlistment);
        }

        Assert(enlistment, Notification.Rollback);
        Transaction.Current.Should().BeNull();
    }

    private static TestEnlistmentNotification Enlist(PrepareAction prepareAction, EnlistmentOptions options)
    {
        var currentTransaction = Transaction.Current;
        if (currentTransaction == null) throw new NullReferenceException();
        var enlistment = new TestEnlistmentNotification(prepareAction);
        currentTransaction.EnlistVolatile(enlistment, options);
        return enlistment;
    }

    private static void Assert(TestEnlistmentNotification enlistment, params Notification[] expectedNotifications)
    {
        enlistment.Notifications.Should().BeEquivalentTo(expectedNotifications);
    }

    public enum PrepareAction
    {
        Prepared,
        Done,
        ForceRollback
    }

    private enum Notification
    {
        Prepare,
        Commit,
        Rollback,
        InDoubt
    }

    private sealed class TestEnlistmentNotification : IEnlistmentNotification
    {
        private readonly PrepareAction _prepareAction;
        private readonly List<Notification> _notifications = new List<Notification>();

        public TestEnlistmentNotification(PrepareAction prepareAction)
        {
            _prepareAction = prepareAction;
        }

        public IEnumerable<Notification> Notifications => _notifications;

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            _notifications.Add(Notification.Prepare);
            switch (_prepareAction)
            {
                case PrepareAction.Prepared:
                    preparingEnlistment.Prepared();
                    return;
                case PrepareAction.Done:
                    preparingEnlistment.Done();
                    return;
                case PrepareAction.ForceRollback:
                    preparingEnlistment.ForceRollback();
                    return;
            }
        }

        public void Commit(Enlistment enlistment)
        {
            _notifications.Add(Notification.Commit);
            enlistment.Done();
        }

        public void Rollback(Enlistment enlistment)
        {
            _notifications.Add(Notification.Rollback);
            enlistment.Done();
        }

        public void InDoubt(Enlistment enlistment)
        {
            _notifications.Add(Notification.InDoubt);
            enlistment.Done();
        }
    }
}