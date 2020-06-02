#!/usr/bin/env bash

GIT_DIR=$(git rev-parse --git-dir)

echo "Installing hooks..."
# this command creates symlink to our pre-push script
ln -s ../../scripts/pre-push.sh $GIT_DIR/hooks/pre-push
ln -s ../../scripts/post-push.sh $GIT_DIR/hooks/post-push
echo "Done!"