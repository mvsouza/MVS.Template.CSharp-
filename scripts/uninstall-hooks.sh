#!/usr/bin/env bash

GIT_DIR=$(git rev-parse --git-dir)

echo "Uninstalling hooks..."
# this command creates symlink to our pre-push script
rm $GIT_DIR/hooks/pre-push
rm $GIT_DIR/hooks/post-push
echo "Done!"