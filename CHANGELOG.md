# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.1] - 2019-12-20

### Changed

- Updated FileData to have two path variables: AbsolutePath and RelativePath to better ease in accessing path data through external scripts. 

## [1.0.0] - 2019-11-26

### Added

- File Data objects that wrap files not serialized by Unity (like those in StreamingAssets) that automatically track and update to their attached file's GUID.
