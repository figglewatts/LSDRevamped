local train = {}
train.currentTarget = 1
train.currentTargetEntity = nil
train.targets = {}

NUM_TRAIN_TARGETS = 91

function train.generateTargetsFrom(startNum)
    if startNum > NUM_TRAIN_TARGETS then
        error("generateTargetsFrom: start number " .. startNum .. " > " .. NUM_TRAIN_TARGETS)
    end

    train.targets = {}
    for i = startNum, NUM_TRAIN_TARGETS do
        local entityName = "TrainTarget" .. tostring(i)
        local entity = GetEntity(entityName)
        table.insert(train.targets, entity)
    end
    train.currentTargetEntity = train.targets[train.currentTarget]
end

function train.incrementTarget(interactiveObject)
    train.currentTarget = train.currentTarget + 1
    if train.currentTarget > #train.targets then
        -- we have reached the end of the target list, stop our action and animation to stop the train
        interactiveObject.StopAnimation()
        interactiveObject.Action.Stop()
        return
    end
    train.currentTargetEntity = train.targets[train.currentTarget]
end

function train.moveTowardsTarget(interactiveObject, moveSpeed)
    local targetPos = train.currentTargetEntity.WorldPosition
    local completed = interactiveObject.MoveTowards(targetPos, moveSpeed)
    interactiveObject.LookTowards(targetPos, 100)

    return completed
end

return train